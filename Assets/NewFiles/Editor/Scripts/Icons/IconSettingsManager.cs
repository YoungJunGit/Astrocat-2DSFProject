using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace NewFiles.Editor
{
    /// <summary>
    /// Aquest script corregeix AUTOMÀTICAMENT la configuració d'importació de les icones
    /// de l'asset just després de la primera importació, evitant que apareguin 
    /// a la finestra "Select Sprite".
    /// Canvia el seu 'Texture Type' a 'Editor GUI and Legacy GUI'.
    /// </summary>
    [InitializeOnLoad] // Fa que el constructor estàtic s'executi quan es carrega l'editor
    public class IconSettingsManager : AssetPostprocessor
    {
        private const string ICON_PATH = "Assets/NewFiles/Editor/Styles/icons";
        private const string ICON_LABEL = "NewFiles Icons";
        
        // Aquesta és la "bandera" per assegurar que la correcció només s'executa UNA vegada.
        private const string FIX_COMPLETED_PREF_KEY = "NewFiles.IconSettingsFixed_v1";

        /// <summary>
        /// Constructor estàtic. S'executa quan es compilen els scripts
        /// (p.ex., just després d'importar el paquet).
        /// </summary>
        static IconSettingsManager()
        {
            // Utilitzem delayCall per esperar que l'editor estigui llest
            EditorApplication.delayCall += CheckAndFixIconSettings;
        }

        /// <summary>
        /// Comprova si la correcció s'ha de fer i l'executa si és necessari.
        /// </summary>
        private static void CheckAndFixIconSettings()
        {
            // 1. Comprova si ja ho hem fet abans
            if (EditorPrefs.GetBool(FIX_COMPLETED_PREF_KEY, false))
            {
                return; // Ja està fet, no facis res.
            }

            // 2. Comprova si la carpeta d'icones existeix
            if (!Directory.Exists(ICON_PATH))
            {
                // Potser l'usuari ha mogut la carpeta, no podem fer res.
                return; 
            }

            // 3. Executa la correcció
            Debug.Log("[NewFiles] Primera execució detectada. Corregint la configuració d'importació de les icones...");
            FixAllIconSettings(true); // Executa la lògica de correcció

            // 4. Desa la "bandera" per no tornar-ho a fer
            EditorPrefs.SetBool(FIX_COMPLETED_PREF_KEY, true);
            Debug.Log("[NewFiles] Les icones s'han configurat correctament i s'amagaran del 'Select Sprite'.");
        }

        /// <summary>
        /// AQUEST MÈTODE S'EXECUTA PRIMER:
        /// S'assegura que QUALSEVOL NOVA icona afegida a la carpeta
        /// s'importi correctament des del principi.
        /// </summary>
        void OnPreprocessTexture()
        {
            if (assetPath.StartsWith(ICON_PATH))
            {
                TextureImporter importer = (TextureImporter)assetImporter;
                if (importer.textureType != TextureImporterType.GUI)
                {
                    importer.textureType = TextureImporterType.GUI;
                    importer.mipmapEnabled = false;
                    importer.isReadable = false;
                }
            }
        }

        /// <summary>
        /// AFEGEIX ETIQUETES:
        /// Això s'executa després de la importació per afegir les etiquetes
        /// i mantenir l'organització (com la "carpeta" que volies).
        /// </summary>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                if (path.StartsWith(ICON_PATH) && AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(Texture2D))
                {
                    AssignLabelToAsset(path);
                }
            }
        }

        private static void AssignLabelToAsset(string assetPath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset == null) return;

            var labels = new List<string>(AssetDatabase.GetLabels(asset));
            if (!labels.Contains(ICON_LABEL))
            {
                labels.Add(ICON_LABEL);
                AssetDatabase.SetLabels(asset, labels.ToArray());
            }
        }

        /// <summary>
        /// L'element de menú manual, per si de cas l'usuari
        /// vol re-executar la correcció.
        /// </summary>
        [MenuItem("Tools/NewFiles/Icon Management/Fix Icon Import Settings (Hide in Picker)")]
        public static void ManualFixAllIconSettings()
        {
            FixAllIconSettings(false); // 'false' vol dir que NO és automàtic
        }

        /// <summary>
        /// La lògica principal que re-importa totes les icones
        /// amb la configuració correcta.
        /// </summary>
        private static void FixAllIconSettings(bool isAutoRun)
        {
            if (!Directory.Exists(ICON_PATH))
            {
                Debug.LogWarning($"[NewFiles] No s'ha trobat la ruta de les icones: {ICON_PATH}");
                return;
            }

            string[] assetGuids = AssetDatabase.FindAssets("t:texture2d", new[] { ICON_PATH });
            int count = 0;
            int fixedCount = 0;

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (string guid in assetGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                    // Només re-importem si la configuració és incorrecta
                    if (importer != null && importer.textureType != TextureImporterType.GUI)
                    {
                        importer.textureType = TextureImporterType.GUI;
                        importer.mipmapEnabled = false;
                        importer.isReadable = false;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        fixedCount++;
                    }
                    count++;
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            string logMessage = $"[NewFiles] S'han processat {count} icones. {fixedCount} icones han estat re-importades amb el tipus 'GUI'.";
            Debug.Log(logMessage);

            // Només mostrem l'avís si l'usuari ho ha fet manualment
            if (!isAutoRun) 
            {
                EditorUtility.DisplayDialog("Configuració d'Icones Corregida",
                    $"S'han processat {count} icones.\n{fixedCount} han estat re-importades correctament.\n\nAra estan configurades com a 'Editor GUI' i ja no apareixeran a la finestra 'Select Sprite'.",
                    "OK");
            }
        }
    }
}