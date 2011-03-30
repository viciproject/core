using System;
using System.Linq;

namespace Vici.Core.Parser.Config
{
    public class ProMeshTokenizer : TemplateTokenizer
    {
        private class ForeachTokenMatcher : WrappedExpressionMatcher
        {
            public ForeachTokenMatcher(params string[] parts)
                : base(parts)
            {
            }

            protected override string TranslateToken(string originalToken, WrappedExpressionMatcher tokenProcessor)
            {
                string s = base.TranslateToken(originalToken, tokenProcessor);

                int inIdx = s.IndexOf(" in ");

                if (inIdx < 0)
                    throw new TemplateParsingException("invalid syntax in foreach", TokenPosition.Unknown);
                else
                    return s.Substring(0, inIdx).Trim() + "\0" + s.Substring(inIdx + 4).Trim();
            }
        }

        public ProMeshTokenizer()
        {
            AddTokenMatcher(TemplateTokenType.ForEach, new ForeachTokenMatcher("<!--{{", "foreach", "}}-->") ,true);
            AddTokenMatcher(TemplateTokenType.ForEach, new ForeachTokenMatcher("<!--$[", "foreach", "]-->"), true);
            AddTokenMatcher(TemplateTokenType.EndBlock, new WrappedExpressionMatcher(false, "<!--{{", "endfor", "}}-->"));
            AddTokenMatcher(TemplateTokenType.EndBlock, new WrappedExpressionMatcher(false, "<!--{{", "endif", "}}-->"));
            AddTokenMatcher(TemplateTokenType.EndBlock, new WrappedExpressionMatcher(false, "<!--{{", "end", "}}-->"));
            AddTokenMatcher(TemplateTokenType.Else, new WrappedExpressionMatcher(false, "<!--{{", "else", "}}-->"));
            AddTokenMatcher(TemplateTokenType.ElseIf, new WrappedExpressionMatcher(false, "<!--{{", "elseif", "}}-->"));
            AddTokenMatcher(TemplateTokenType.If, new WrappedExpressionMatcher(false, "<!--{{", "if", "}}-->"));
            AddTokenMatcher(TemplateTokenType.MacroDefinition, new WrappedExpressionMatcher(false, "<!--{{", "macro", "}}-->"));
            AddTokenMatcher(TemplateTokenType.MacroCall, new WrappedExpressionMatcher("<!--{{", "call", "}}-->"));
            AddTokenMatcher(TemplateTokenType.Statement, new WrappedExpressionMatcher(false, "<!--{{", "}}-->"));
            AddTokenMatcher(TemplateTokenType.Expression, new WrappedExpressionMatcher(false, "{{", "}}"));

            AddTokenMatcher(TemplateTokenType.EndBlock, new WrappedExpressionMatcher(false, "<!--$[", "endfor", "]-->"));
            AddTokenMatcher(TemplateTokenType.EndBlock, new WrappedExpressionMatcher(false, "<!--$[", "endif", "]-->"));
            AddTokenMatcher(TemplateTokenType.EndBlock, new WrappedExpressionMatcher(false, "<!--$[", "end", "]-->"));
            AddTokenMatcher(TemplateTokenType.Else, new WrappedExpressionMatcher(false, "<!--$[", "else", "]-->"));
            AddTokenMatcher(TemplateTokenType.ElseIf, new WrappedExpressionMatcher(false, "<!--$[", "elseif", "]-->"));
            AddTokenMatcher(TemplateTokenType.If, new WrappedExpressionMatcher(false, "<!--$[", "if", "]-->"));
            AddTokenMatcher(TemplateTokenType.MacroDefinition, new WrappedExpressionMatcher(false, "<!--$[", "macro", "]-->"));
            AddTokenMatcher(TemplateTokenType.MacroCall, new WrappedExpressionMatcher("<!--$[", "call", "]-->"));
            AddTokenMatcher(TemplateTokenType.Statement, new WrappedExpressionMatcher(false, "<!--$[", "]-->"));
            AddTokenMatcher(TemplateTokenType.Expression, new WrappedExpressionMatcher(false, "$[", "]"));
        }
    }
}