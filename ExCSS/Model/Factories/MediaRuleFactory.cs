﻿using System.Collections.Generic;
using ExCSS.Model.Extensions;

namespace ExCSS.Model.Factories
{
    internal class MediaRuleFactory : RuleFactory
    {
        public MediaRuleFactory(StyleSheetContext context) : base(context)
        {}

        public override void Parse(IEnumerator<Block> reader)
        {
            var media = new MediaRule(Context);
            Context.ActiveRules.Push(media);

            AppendMediaList(Context, reader, media.Media, GrammarSegment.CurlyBraceOpen);

            if (reader.Current.GrammarSegment == GrammarSegment.CurlyBraceOpen)
            {
                if (reader.SkipToNextNonWhitespace())
                {
                    reader.LimitToCurrentBlock();
                    Context.BuildRulesets(media.Declarations);
                }
            }

            Context.ActiveRules.Pop();
            Context.AtRules.Add(media);
        }

        internal static void AppendMediaList(StyleSheetContext context, IEnumerator<Block> reader, MediaTypes media, 
            GrammarSegment endToken = GrammarSegment.Semicolon)
        {
            var firstPass = true;
            do
            {
                if (reader.Current.GrammarSegment == GrammarSegment.Whitespace)
                {
                    continue;
                }

                if (reader.Current.GrammarSegment == endToken)
                {
                    break;
                }

                do
                {
                    if (reader.Current.GrammarSegment == GrammarSegment.Comma || reader.Current.GrammarSegment == endToken)
                    {
                        break;
                    }

                    if (reader.Current.GrammarSegment == GrammarSegment.Whitespace)
                    {
                        context.ReadBuffer.Append(' ');
                    }
                    else
                    {
                        if (!firstPass)
                        {
                            context.ReadBuffer.Append(reader.Current);
                        }
                        firstPass = false;
                    }
                }
                while (reader.MoveNext());

                if (context.ReadBuffer.Length > 0)
                {
                    media.AppendMedium(context.ReadBuffer.ToString());
                }

                context.ReadBuffer.Clear();

                if (reader.Current.GrammarSegment == endToken)
                {
                    break;
                }
            }
            while (reader.MoveNext());
        }
    }
}