using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetOffice.WordApi;
using NetOffice.WordApi.Enums;

namespace XProof
{
    class Word
    {
        public static void Open(string filename)
        {
            using (var word = new Application() { DisplayAlerts = WdAlertLevel.wdAlertsNone, Visible = false })
            {
#if DEBUG
                word.Visible = true;
#endif
                var doc = word.Documents.Open(filename);
                word.ActiveWindow.View.Draft = true;
                doc.Select();
                doc.GrammarChecked = false;
                doc.SpellingChecked = false;
                word.Selection.Move();
                word.Visible = true;
                doc.CheckGrammar();
            }
        }
    }
}
