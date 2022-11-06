using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TemplateWizard;
using System.Windows.Forms;
using EnvDTE;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Verse3LibraryTemplate
{
    public class WizardImplementation : IWizard
    {
        private UserInputForm inputForm;
        private string libraryName;
        private string libraryAuthor;
        private string compName;
        private string compGroup;
        private string compTab;

        // This method is called before opening any item that
        // has the OpenInEditor attribute.
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        // This method is only called for item templates,
        // not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem
            projectItem)
        {
        }

        // This method is called after the project is created.
        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            try
            {
                // Display a form to the user. The form collects
                // input for the custom message.
                inputForm = new UserInputForm();
                inputForm.ShowDialog();

                //customMessage = UserInputForm.CustomMessage;
                if (inputForm.DialogResult == DialogResult.OK)
                {
                    libraryName = inputForm.LibraryName;
                    libraryAuthor = inputForm.LibraryAuthor;
                    compName = inputForm.CompName;
                    compGroup = inputForm.CompGroup;
                    compTab = inputForm.CompTab;
                }
                else
                {
                    return;
                }

                // Add custom parameters.
                replacementsDictionary.Add("$libraryname$", libraryName);
                replacementsDictionary.Add("$libraryauthor$", libraryAuthor);
                replacementsDictionary.Add("$compname$", compName);
                replacementsDictionary.Add("$compgroup$", compGroup);
                replacementsDictionary.Add("$comptab$", compTab);
                string className = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(compName);
                bool isValid = Microsoft.CSharp.CSharpCodeProvider.CreateProvider("C#").IsValidIdentifier(className);

                if (!isValid)
                {
                    // File name contains invalid chars, remove them
                    Regex regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
                    className = regex.Replace(className, "");

                    // Class name doesn't begin with a letter, insert an underscore
                    if (!char.IsLetter(className, 0))
                    {
                        className = className.Insert(0, "_");
                    }
                }
                replacementsDictionary.Add("$safecompname$", (className + "Comp"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // This method is only called for item templates,
        // not for project templates.
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        void IWizard.RunFinished()
        {
            throw new NotImplementedException();
        }

        void IWizard.RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            throw new NotImplementedException();
        }

        bool IWizard.ShouldAddProjectItem(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}