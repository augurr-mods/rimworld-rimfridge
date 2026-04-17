using RimWorld;
using UnityEngine;
using Verse;

namespace RimFridge
{
    /// <summary>
    /// Custom rename window: vanilla rename dialogs were refactored (generic, Thing-centric) while this mod keeps a separate label on <see cref="CompRefrigerator"/>.
    /// </summary>
    public class Dialog_RenameFridge : Window
    {
        private const int MaxNameLength = 28;

        private readonly CompRefrigerator fridge;
        private string curName;
        private bool focusedRenameField;

        public Dialog_RenameFridge(CompRefrigerator fridge)
        {
            this.fridge = fridge;
            curName = string.IsNullOrEmpty(fridge.buildingLabel) ? fridge.parent.Label : fridge.buildingLabel;

            forcePause = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnClickedOutside = true;
        }

        public override Vector2 InitialSize => new Vector2(280f, 225f);

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            bool accept = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                accept = true;
                Event.current.Use();
            }

            GUI.SetNextControlName("RenameField");
            string text = Widgets.TextField(new Rect(0f, 15f, inRect.width, 35f), curName);
            if (text.Length < MaxNameLength)
            {
                curName = text;
            }

            if (!focusedRenameField)
            {
                UI.FocusControl("RenameField", this);
                focusedRenameField = true;
            }

            float okY = inRect.height - 35f - 15f;
            if (Widgets.ButtonText(new Rect(15f, okY - 50f, inRect.width - 30f, 35f), "ResetButton".Translate(), true, false, true))
            {
                fridge.buildingLabel = "";
                Find.WindowStack.TryRemove(this, true);
                return;
            }

            if (Widgets.ButtonText(new Rect(15f, okY, inRect.width - 30f, 35f), "OK".Translate(), true, false, true) || accept)
            {
                AcceptanceReport report = NameIsValid(curName);
                if (!report.Accepted)
                {
                    if (!report.Reason.NullOrEmpty())
                    {
                        Messages.Message(report.Reason, MessageTypeDefOf.RejectInput, false);
                    }
                    else
                    {
                        Messages.Message("NameIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                    }
                }
                else
                {
                    fridge.buildingLabel = curName;
                    Find.WindowStack.TryRemove(this, true);
                }
            }
        }

        private AcceptanceReport NameIsValid(string name)
        {
            if (name.Length == 0)
            {
                return true;
            }

            if (name.Length > MaxNameLength)
            {
                return new AcceptanceReport("NameTooLong".Translate());
            }

            return AcceptanceReport.WasAccepted;
        }
    }
}
