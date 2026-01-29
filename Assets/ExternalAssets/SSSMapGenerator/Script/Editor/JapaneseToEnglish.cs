/*
 * [About this class]
 * This class is designed for English-speaking users and removes Japanese comments from scripts.
 * By selecting "Menu > Tools > SSSMapGenerator > Japanese to English", you can remove all Japanese comments from every script.
 * [このclassについて]
 * これは英語圏の方の為に作られた、スクリプトから日本語のコメントを削除するclassです
 * 「メニュー＞Tools＞SSSMapGenerator＞Japanese to English」を選択すれば全てのスクリプトから日本語のコメントを削除できます
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

//名前空間｜Namespace
namespace S3MG{

    public class JapaneseToEnglish : EditorWindow{

        [MenuItem("Tools/SSSMapGenerator/Japanese to English")]
        static void ShowWindow(){
            //確認ダイアログを表示
            bool confirmed = EditorUtility.DisplayDialog(
                "Remove Japanese Comments Confirmation",
                "Do you want to remove all Japanese comments from the scripts in the Assets/SSSMapGenerator/Script folder?\n" +
                "This action cannot be undone.\n\n" +
                "Process details:\n" +
                "- Remove Japanese and replace with English",
                "Execute",
                "Cancel"
            );
            if(confirmed){
                ProcessSSSMapGeneratorScripts();
            }
        }

        static void ProcessSSSMapGeneratorScripts(){
            //対象フォルダのパスを設定
            string targetFolderPath = "Assets/SSSMapGenerator/Script";

            //フォルダが存在するかチェック
            if(!AssetDatabase.IsValidFolder(targetFolderPath)){
                EditorUtility.DisplayDialog("Error", $"Target folder not found: {targetFolderPath}", "OK");
                return;
            }

            //フォルダ内の全てのスクリプトファイルを取得（サブフォルダを含む）
            string[] allScriptFiles = Directory.GetFiles(Path.Combine(Application.dataPath, "SSSMapGenerator/Script"), "*.cs", SearchOption.AllDirectories);

            //Editorフォルダ内のファイルを除外
            List<string> scriptFiles = new List<string>();
            foreach(string filePath in allScriptFiles){
                //ファイルパスにEditorフォルダが含まれているかチェック
                if(!IsInEditorFolder(filePath)){
                    scriptFiles.Add(filePath);
                }
            }

            int processedCount = 0;
            int excludedCount = allScriptFiles.Length - scriptFiles.Count;

            foreach (string filePath in scriptFiles){
                //Unityのアセットパス形式に変換
                string relativePath = "Assets" + filePath.Substring(Application.dataPath.Length);

                ProcessScriptFile(relativePath);
                processedCount++;
            }

            if(processedCount > 0){
                AssetDatabase.Refresh();
                string message = $"{processedCount} script files processed.";
                if(excludedCount > 0){
                    message += $"\n{excludedCount} files in Editor folders were excluded.";
                }
                EditorUtility.DisplayDialog("Processing complete", message, "OK");
            }else{
                EditorUtility.DisplayDialog("Notice", "No script files found to process.", "OK");
            }
        }

        //ファイルがEditorフォルダ内にあるかチェック
        static bool IsInEditorFolder(string filePath){
            //パスを正規化してチェック（大文字小文字を区別しない）
            string normalizedPath = filePath.Replace('\\', '/').ToLower();

            //パスの区切り文字でフォルダ名を分割
            string[] pathParts = normalizedPath.Split('/');

            //フォルダ名のリストに"editor"が含まれているかチェック
            return pathParts.Contains("editor");
        }

        static void ProcessScriptFile(string filePath){
            try{
                string content = File.ReadAllText(filePath);
                string processedContent = RemoveJapaneseComments(content);
                File.WriteAllText(filePath, processedContent);
                Debug.Log($"Processing complete: {filePath}");
            }catch (Exception e){
                Debug.LogError($"File processing error: {filePath}\n{e.Message}");
            }
        }

        static string RemoveJapaneseComments(string content){
            //1. [Header(...)]アトリビュートの処理
            content = ProcessHeaderAttributes(content);

            //2. /* ... */コメントの処理（複数行対応）
            content = ProcessMultiLineComments(content);

            //3. Debug.Logの処理
            content = ProcessDebugLogs(content);

            //4. //コメントの処理
            content = ProcessSingleLineComments(content);

            //5. 複数連続改行を1つにまとめる
            content = ReduceMultipleEmptyLines(content);

            return content;
        }

        static string ProcessHeaderAttributes(string content){
            //[Header("...")]パターンをマッチ
            string pattern = @"\[Header\(""([^""]+)""\)\]";

            return Regex.Replace(content, pattern, (Match match) => {
                string headerText = match.Groups[1].Value;

                //▼を保持するために分離
                string prefix = "";
                string mainText = headerText;
                if(headerText.StartsWith("▼")){
                    prefix = "▼";
                    mainText = headerText.Substring(1);
                }

                //日本語｜英語パターンをチェック
                if(mainText.Contains("｜")){
                    string[] parts = mainText.Split('｜');
                    if(parts.Length >= 2){
                        //日本語部分を削除し、英語部分のみ保持
                        string englishPart = parts[1].Trim();
                        return $"[Header(\"{prefix}{englishPart}\")]";
                    }
                }

                //日本語のみかチェック（ひらがな、カタカナ、漢字を含むかチェック）
                if(ContainsJapanese(mainText)){
                    //日本語のみの場合は削除
                    return $"[Header(\"{prefix}\")]";
                }

                //変更なし
                return match.Value;
            });
        }

        static string ProcessMultiLineComments(string content){
            ///* ... */パターンをマッチ（複数行対応）
            string pattern = @"/\*(.*?)\*/";

            return Regex.Replace(content, pattern, (Match match) => {
                string commentText = match.Groups[1].Value;

                //コメント内にコードが含まれているかチェック（セミコロンや波括弧の存在で判定）
                bool containsCode = commentText.Contains(";") || commentText.Contains("{") || commentText.Contains("}");

                //見出し形式（----で囲まれている）かチェック
                bool isHeaderFormat = commentText.Contains("----");

                if(containsCode){
                    //コード含有の場合：内部の//コメントのみ処理
                    string processedComment = ProcessSingleLineCommentsInMultiLineComment(commentText);
                    return $"/*{processedComment}*/";
                }else if(isHeaderFormat){
                    //見出し形式の場合：日本語：英語パターンを処理し、構造は保持
                    string processedComment = ProcessHeaderFormatComment(commentText);
                    return $"/*{processedComment}*/";
                }else if(ContainsJapanese(commentText)){
                    //通常の日本語コメントの場合は空のコメントに
                    return "/**/";
                }

                //変更なし
                return match.Value;
            }, RegexOptions.Singleline);
        }

        //複数行コメント内の//コメントのみを処理
        static string ProcessSingleLineCommentsInMultiLineComment(string content){
            string[] lines = content.Split('\n');
            List<string> resultLines = new List<string>();

            foreach (string line in lines){
                if(line.TrimStart().StartsWith("//")){
                    //行全体が//コメントの場合
                    string trimmedLine = line.TrimStart();
                    string commentPart = trimmedLine.Substring(2).Trim();

                    //TODO行は保持
                    if(commentPart.StartsWith("TODO:") || commentPart.StartsWith("TODO｜")){
                        resultLines.Add(line);
                        continue;
                    }

                    //日本語｜英語パターンをチェック
                    if(commentPart.Contains("｜")){
                        string[] parts = commentPart.Split('｜');
                        if(parts.Length >= 2){
                            //前部分が日本語、後部分が英語の場合は英語部分のみ保持
                            bool frontIsJapanese = ContainsJapanese(parts[0]);
                            bool backIsJapanese = ContainsJapanese(parts[1]);

                            if(frontIsJapanese && !backIsJapanese){
                                //英語部分のみ保持（インデントも保持）
                                string indent = line.Substring(0, line.Length - trimmedLine.Length);
                                resultLines.Add($"{indent}//{parts[1].Trim()}");
                                continue;
                            }
                        }
                    }

                    //日本語が含まれているかチェック
                    if(ContainsJapanese(commentPart)){
                        //日本語が含まれている場合は行ごとスキップ
                        continue;
                    }else{
                        //英語のみのコメントはそのまま追加
                        resultLines.Add(line);
                    }
                }else if(line.Contains("//")){
                    //コードの後ろにコメントがある場合
                    int commentIndex = line.IndexOf("//");
                    string codePart = line.Substring(0, commentIndex).TrimEnd();
                    string commentPart = line.Substring(commentIndex + 2).Trim();

                    //TODO行は保持
                    if(commentPart.StartsWith("TODO:") || commentPart.StartsWith("TODO｜")){
                        resultLines.Add(line);
                        continue;
                    }

                    //日本語｜英語パターンをチェック
                    if(commentPart.Contains("｜")){
                        string[] parts = commentPart.Split('｜');
                        if(parts.Length >= 2){
                            //前部分が日本語、後部分が英語の場合は英語部分のみ保持
                            bool frontIsJapanese = ContainsJapanese(parts[0]);
                            bool backIsJapanese = ContainsJapanese(parts[1]);

                            if(frontIsJapanese && !backIsJapanese){
                                //英語部分のみコメントとして追加
                                resultLines.Add($"{codePart}//{parts[1].Trim()}");
                                continue;
                            }
                        }
                    }

                    //それ以外はコメント部分を削除してコードだけ残す
                    resultLines.Add(codePart);
                }else{
                    //通常の行はそのまま追加
                    resultLines.Add(line);
                }
            }

            return string.Join("\n", resultLines);
        }

        //見出し形式のコメントを処理
        static string ProcessHeaderFormatComment(string content){
            string[] lines = content.Split('\n');
            List<string> resultLines = new List<string>();
            bool hasContent = false; //内容があるかどうかのフラグ

            foreach (string line in lines){
                string trimmedLine = line.Trim();

                //----の行はそのまま保持
                if(trimmedLine.StartsWith("----")){
                    resultLines.Add(line);
                }else if(ContainsJapanese(trimmedLine)){
                    //日本語が含まれる行を処理
                    if(trimmedLine.Contains("｜")){
                        //日本語｜英語パターンの場合
                        string[] parts = trimmedLine.Split('｜');
                        if(parts.Length >= 2){
                            bool frontIsJapanese = ContainsJapanese(parts[0]);
                            bool backIsJapanese = ContainsJapanese(parts[1]);

                            if(frontIsJapanese && !backIsJapanese){
                                //前が日本語、後が英語の場合は英語部分のみ保持（インデントも保持）
                                string indent = line.Substring(0, line.Length - line.TrimStart().Length);
                                resultLines.Add(indent + parts[1].Trim());
                                hasContent = true;
                            }else{
                                //前後とも日本語の場合は空行に（インデントは保持）
                                string indent = line.Substring(0, line.Length - line.TrimStart().Length);
                                resultLines.Add(indent);
                            }
                        }else{
                            //分割できない場合は空行に
                            string indent = line.Substring(0, line.Length - line.TrimStart().Length);
                            resultLines.Add(indent);
                        }
                    }else{
                        //日本語のみの場合は空行に（インデントは保持）
                        string indent = line.Substring(0, line.Length - line.TrimStart().Length);
                        resultLines.Add(indent);
                    }
                }else if(!string.IsNullOrWhiteSpace(trimmedLine)){
                    //英語のみの行はそのまま保持
                    resultLines.Add(line);
                    hasContent = true;
                }else{
                    //空行はそのまま保持
                    resultLines.Add(line);
                }
            }

            //内容がない場合（----行のみの場合）は空行を削除して詰める
            if(!hasContent){
                List<string> compactLines = new List<string>();
                foreach (string line in resultLines){
                    string trimmedLine = line.Trim();
                    if(trimmedLine.StartsWith("----") || trimmedLine.Length == 0){
                        compactLines.Add(line);
                    }
                }

                //連続する空行を削除
                List<string> finalLines = new List<string>();
                bool lastWasEmpty = false;
                foreach (string line in compactLines){
                    string trimmedLine = line.Trim();
                    if(string.IsNullOrWhiteSpace(trimmedLine)){
                        if(!lastWasEmpty && !trimmedLine.StartsWith("----")){
                            //----行ではない空行で、前の行も空行でない場合のみ追加しない
                            lastWasEmpty = true;
                            continue;
                        }
                    }else{
                        lastWasEmpty = false;
                    }
                    finalLines.Add(line);
                }
                return string.Join("\n", finalLines);
            }

            return string.Join("\n", resultLines);
        }

        static string ProcessDebugLogs(string content){
            //Debug.Log/LogWarning/LogError(...);パターンをマッチ（複数行対応、ネストした括弧も考慮）
            string pattern = @"Debug\.(Log|LogWarning|LogError)\s*\(\s*([^;]+?)\s*\)\s*;";

            return Regex.Replace(content, pattern, (Match match) => {
                string logType = match.Groups[1].Value;//Log, LogWarning, LogError
                string logContent = match.Groups[2].Value;

                //文字列リテラル内の日本語を処理
                logContent = ProcessStringLiterals(logContent);

                return $"Debug.{logType}({logContent});";
            }, RegexOptions.Singleline);
        }

        static string ProcessStringLiterals(string content){
            //文字列リテラル（$"...", "..."）をマッチ
            string pattern = @"(\$?""[^""]*"")";

            return Regex.Replace(content, pattern, (Match match) => {
                string stringLiteral = match.Groups[1].Value;

                //クォート内の文字列を取得
                string innerContent;
                if(stringLiteral.StartsWith("$\"")){
                    innerContent = stringLiteral.Substring(2, stringLiteral.Length - 3);//$"と"を除去
                }else{
                    innerContent = stringLiteral.Substring(1, stringLiteral.Length - 2);//"と"を除去
                }

                string processedContent = innerContent;

                //日本語｜英語パターンをチェック
                if(innerContent.Contains("｜")){
                    string[] parts = innerContent.Split('｜');
                    if(parts.Length >= 2){
                        //前部分が日本語、後部分も日本語かチェック
                        bool frontIsJapanese = ContainsJapanese(parts[0]);
                        bool backIsJapanese = ContainsJapanese(parts[1]);

                        if(frontIsJapanese && backIsJapanese){
                            //前後とも日本語の場合は全て削除
                            processedContent = "";
                        }else if(frontIsJapanese && !backIsJapanese){
                            //前が日本語、後が英語の場合は英語部分のみ保持
                            processedContent = parts[1].Trim();
                        }else{
                            //変更なし
                            processedContent = innerContent;
                        }
                    }else if(ContainsJapanese(innerContent)){
                        //｜があるが分割できない場合で日本語が含まれている場合は削除
                        processedContent = "";
                    }
                }else if(ContainsJapanese(innerContent)){
                    //日本語のみの場合は削除
                    processedContent = "";
                }

                //元の形式で返す
                if(stringLiteral.StartsWith("$\"")){
                    return $"$\"{processedContent}\"";
                }else{
                    return $"\"{processedContent}\"";
                }
            });
        }

        static string ProcessSingleLineComments(string content){
            string[] lines = content.Split('\n');
            List<string> resultLines = new List<string>();

            for (int i = 0; i < lines.Length; i++){
                string line = lines[i];

                //行全体が//コメントの場合
                string trimmedLine = line.TrimStart();
                if(trimmedLine.StartsWith("//")){
                    string commentPart = trimmedLine.Substring(2).Trim();

                    //TODO行は保持
                    if(commentPart.StartsWith("TODO:") || commentPart.StartsWith("TODO｜")){
                        resultLines.Add(line);
                        continue;
                    }

                    //日本語｜英語パターンをチェック
                    if(commentPart.Contains("｜")){
                        string[] parts = commentPart.Split('｜');
                        if(parts.Length >= 2){
                            //前部分が日本語、後部分が英語の場合は英語部分のみ保持
                            bool frontIsJapanese = ContainsJapanese(parts[0]);
                            bool backIsJapanese = ContainsJapanese(parts[1]);

                            if(frontIsJapanese && !backIsJapanese){
                                //英語部分のみ保持（インデントも保持）
                                string indent = line.Substring(0, line.Length - trimmedLine.Length);
                                resultLines.Add($"{indent}//{parts[1].Trim()}");
                                continue;
                            }
                        }
                    }

                    //日本語が含まれているかチェック
                    if(ContainsJapanese(commentPart)){
                        //日本語が含まれている場合は行ごとスキップ（追加しない）
                        continue;
                    }else{
                        //英語のみのコメントはそのまま追加
                        resultLines.Add(line);
                    }
                }else if(line.Contains("//")){
                    //スクリプトの後ろにコメントがある場合
                    int commentIndex = line.IndexOf("//");
                    string codePart = line.Substring(0, commentIndex).TrimEnd();
                    string commentPart = line.Substring(commentIndex + 2).Trim();

                    //TODO行は保持
                    if(commentPart.StartsWith("TODO:") || commentPart.StartsWith("TODO｜")){
                        resultLines.Add(line);
                        continue;
                    }

                    //日本語｜英語パターンをチェック
                    if(commentPart.Contains("｜")){
                        string[] parts = commentPart.Split('｜');
                        if(parts.Length >= 2){
                            //前部分が日本語、後部分が英語の場合は英語部分のみ保持
                            bool frontIsJapanese = ContainsJapanese(parts[0]);
                            bool backIsJapanese = ContainsJapanese(parts[1]);

                            if(frontIsJapanese && !backIsJapanese){
                                //英語部分のみコメントとして追加
                                resultLines.Add($"{codePart}//{parts[1].Trim()}");
                                continue;
                            }
                        }
                    }

                    //それ以外はコメント部分を削除してコードだけ残す
                    resultLines.Add(codePart);
                }else{
                    //通常の行はそのまま追加
                    resultLines.Add(line);
                }
            }

            return string.Join("\n", resultLines);
        }

        //複数の連続する空行を1つの空行にまとめる
        static string ReduceMultipleEmptyLines(string content){
            //3つ以上の連続する改行を2つ（空行1つ）に置換
            return Regex.Replace(content, @"(\r?\n\s*){3,}", "$1$1");
        }

        //日本語文字（ひらがな、カタカナ、漢字、全角文字）が含まれているかチェック
        static bool ContainsJapanese(string text){
            //ひらがな、カタカナ、漢字、全角記号の範囲をチェック：Unicode範囲指定で確実に判定
            return Regex.IsMatch(text, @"[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FFF\uFF00-\uFFEF]");
        }

    }

}
