// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// There are currently no situations in which this rule will fire.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This rule has no behavior by design.")]
    [NoCodeFix("Don't fix what isn't broken.")]
    internal class SA1301ElementMustBeginWithLowerCaseLetter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1301ElementMustBeginWithLowerCaseLetter"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1301";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1301.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1301Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1301MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1301Description), NamingResources.ResourceManager, typeof(NamingResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable RS2000 // Add analyzer diagnostic IDs to analyzer release.
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Error, AnalyzerConstants.EnabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);
#pragma warning restore RS2000 // Add analyzer diagnostic IDs to analyzer release.

        private static readonly Action<SyntaxNodeAnalysisContext> LocalFunctionStatementAction = HandleLocalFunctionStatement;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(LocalFunctionStatementAction, SyntaxKindEx.LocalFunctionStatement);

            // Intentionally empty
        }

        private static void HandleLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntaxWrapper)context.Node;
            CheckElementNameToken(context, localFunctionStatement.Identifier);
        }

        private static void CheckElementNameToken(SyntaxNodeAnalysisContext context, SyntaxToken identifier, bool allowUnderscoreDigit = false)
        {
            if (identifier.IsMissing)
            {
                return;
            }

            if (string.IsNullOrEmpty(identifier.ValueText))
            {
                return;
            }

            if (char.IsLower(identifier.ValueText[0]) && identifier.ValueText[0] != '_')
            {
                return;
            }

            if (allowUnderscoreDigit && (identifier.ValueText.Length > 1) && (identifier.ValueText[0] == '_') && char.IsDigit(identifier.ValueText[1]))
            {
                return;
            }

            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(context.Node))
            {
                return;
            }

            var symbolInfo = context.SemanticModel.GetDeclaredSymbol(identifier.Parent);
            if (symbolInfo != null && NamedTypeHelpers.IsImplementingAnInterfaceMember(symbolInfo))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), identifier.ValueText));
        }
    }
}
