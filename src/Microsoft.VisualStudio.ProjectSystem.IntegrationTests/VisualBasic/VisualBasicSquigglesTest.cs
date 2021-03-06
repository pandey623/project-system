﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.IntegrationTest.Utilities;
using Xunit;

namespace Microsoft.VisualStudio.ProjectSystem.IntegrationTests
{
    [Collection(nameof(SharedIntegrationHostFixture))]
    public class VisualBasicSquigglesTest : AbstractIntegrationTest
    {
        protected override string DefaultLanuageName => LanguageNames.VisualBasic;

        public VisualBasicSquigglesTest(VisualStudioInstanceFactory instanceFactory)
            : base(nameof(CSharpSquigglesTests), WellKnownProjectTemplates.VisualBasicNetCoreClassLibrary, instanceFactory)
        {
            VisualStudio.SolutionExplorer.OpenFile(Project, "Class1.vb");
        }

        [Fact, Trait("Integration", "Squiggles")]
        public void VerifySyntaxErrorSquiggles()
        {
            VisualStudio.Editor.SetText(@"Class A
      Sub S()
        Dim x = 1 +
      End Sub
End Class");

            VisualStudio.Workspace.WaitForAsyncOperations(FeatureAttribute.SolutionCrawler);
            VisualStudio.Workspace.WaitForAsyncOperations(FeatureAttribute.DiagnosticService);
            var actualTags = VisualStudio.Editor.GetErrorTags();
            var expectedTags = new[]
            {
               "Microsoft.VisualStudio.Text.Tagging.ErrorTag:'\r'[43-44]",
               "Microsoft.VisualStudio.Text.Tagging.ErrorTag:'x'[36-37]"
            };
            Assert.Equal(expectedTags, actualTags);
        }

        [Fact, Trait("Integration", "Squiggles")]
        public void VerifySemanticErrorSquiggles()
        {
            VisualStudio.Editor.SetText(@"Class A
      Sub S(b as Bar)
      End Sub
End Class");
            VisualStudio.Editor.Verify.ErrorTags("Microsoft.VisualStudio.Text.Tagging.ErrorTag:'Bar'[26-29]");
        }
    }
}
