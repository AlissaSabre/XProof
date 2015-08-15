using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetOffice.WordApi;
using NetOffice.WordApi.Enums;

namespace XProof
{
    class Word : IDisposable
    {
        private Application WordApp = new Application()
        {
            DisplayAlerts = WdAlertLevel.wdAlertsNone,
#if DEBUG
            Visible = true,
#else
            Visible = false,
#endif
        };

        public void Dispose()
        {
            var word = WordApp;
            WordApp = null;
            if (word != null)
            {
                try
                {
                    word.Visible = false;
                    word.Quit(WdSaveOptions.wdDoNotSaveChanges);
                }
                catch (Exception) { }
                word.Dispose();
            }
        }

        public void Proofread(string filename)
        {
            var word = WordApp;

            var doc = word.Documents.Open(filename);
            word.ActiveWindow.View.Draft = true;
            doc.Select();
            doc.GrammarChecked = false;
            doc.SpellingChecked = false;
            word.Selection.Move();
            word.Visible = true;
            doc.CheckGrammar();

            word.DisposeChildInstances();
        }
    }
}
