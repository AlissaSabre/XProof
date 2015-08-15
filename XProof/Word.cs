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
        private Application WordApp = null;

        public void Dispose()
        {
            var word = WordApp;
            WordApp = null;
            if (word != null)
            {
                word.Dispose();
            }
        }

        public void Open(string filename)
        {
            if (WordApp == null)
            {
                WordApp = new Application();
                WordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;
#if DEBUG
                WordApp.Visible = true;
#else
                WordApp.Visible = false;
#endif
            };

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

        public void Shutdown()
        {
            var word = WordApp;
            if (word != null)
            {
                word.ActiveDocument.Close();
                word.DisposeChildInstances();
            }
        }
    }
}
