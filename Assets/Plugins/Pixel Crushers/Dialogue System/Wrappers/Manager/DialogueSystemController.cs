// Copyright (c) Pixel Crushers. All rights reserved.

using System;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.Wrappers
{

    /// <summary>
    /// This wrapper class keeps references intact if you switch between the 
    /// compiled assembly and source code versions of the original class.
    /// </summary>
    [HelpURL("https://pixelcrushers.com/dialogue_system/manual2x/html/dialogue_system_controller.html")]
    [AddComponentMenu("Pixel Crushers/Dialogue System/Misc/Dialogue System Controller")]
    public class DialogueSystemController : PixelCrushers.DialogueSystem.DialogueSystemController
    {
        public static Action OnDialogueEnd;

        public void EndDialogue()
        {
            OnDialogueEnd.Invoke();
        }
    }

}
