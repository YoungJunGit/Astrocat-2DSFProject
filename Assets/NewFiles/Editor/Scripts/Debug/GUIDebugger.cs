using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace NewFiles.Editor
{
    /// <summary>
    /// A utility tool for debugging GUI stack imbalances within the Unity editor.
    /// It helps ensure that every 'Begin' call in GUI code has a corresponding 'End' call,
    /// preventing common 'GUIClip' errors caused by a corrupted stack.
    /// </summary>
    public static class GUIDebugger
    {
        private static Stack<string> guiStateStack = new Stack<string>();

        /// <summary>
        /// Marks the beginning of a GUI block to be debugged.
        /// </summary>
        /// <param name="blockName">A descriptive name for the GUI block (e.g., "MainToolbar").</param>
        [Conditional("UNITY_EDITOR")]
        public static void Begin(string blockName)
        {
            guiStateStack.Push(blockName);
        }

        /// <summary>
        /// Marks the end of a GUI block and validates it against the last opened block.
        /// Throws an error if the block name does not match the last one on the stack.
        /// </summary>
        /// <param name="blockName">The same name that was used in the corresponding Begin() call.</param>
        [Conditional("UNITY_EDITOR")]
        public static void End(string blockName)
        {
            if (guiStateStack.Count == 0)
            {
                UnityEngine.Debug.LogError($"[GUIDebug] ERROR: Attempting to pop '{blockName}', but the stack is empty. " +
                                 $"This indicates an 'End' call without a corresponding 'Begin'.");
                return;
            }

            string lastBlock = guiStateStack.Pop();
            if (lastBlock != blockName)
            {
                UnityEngine.Debug.LogError($"[GUIDebug] MISMATCH ERROR: Expected end of block '{lastBlock}', but found '{blockName}'. " +
                                 $"The GUI stack is now corrupt.");
            }
        }

        /// <summary>
        /// Checks for any unclosed GUI blocks at the end of a drawing cycle.
        /// This should be called at the end of an OnGUI method to ensure the stack is clean.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void CheckCleanState()
        {
            if (guiStateStack.Count > 0)
            {
                string openBlocks = string.Join(", ", guiStateStack.ToArray());
                UnityEngine.Debug.LogError($"[GUIDebug] ERROR: At the end of the GUI cycle, the following blocks were left open: {openBlocks}. " +
                                 "This will cause GUIClip errors.");
                guiStateStack.Clear();
            }
        }
    }
}