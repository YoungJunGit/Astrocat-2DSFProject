using UnityEngine;
using Ink.Runtime;

namespace PixelCrushers.DialogueSystem.InkSupport
{

    public class BarkInkOnIdle : BarkOnIdle
    {

        [Tooltip("Bark this knot. Can also specify 'knot.stitch' to jump to a stitch in the knot. If blank, bark from the beginning.")]
        public string barkKnot;

        private int progress = 0;

        public override void TryBark(Transform target, Transform interactor)
        {
            Story story = DialogueSystemInkIntegration.LookupStory(conversation);
            if (story == null) return;
            story.ResetState();
            if (!string.IsNullOrEmpty(barkKnot))
            {
                story.ChoosePathString(barkKnot);
            }
            if (!story.canContinue) return;

            var barkText = story.Continue().Trim();
            if (barkOrder == BarkOrder.Sequential)
            {
                for (int i = 0; i < progress; i++)
                {
                    if (!story.canContinue)
                    {
                        break;
                    }
                    barkText = story.Continue().Trim();
                }
                if (story.canContinue)
                {
                    progress++;
                }
                else
                {
                    progress = 0;
                }
            }

            DialogueManager.BarkString(barkText, GetBarker());
        }

    }
}
