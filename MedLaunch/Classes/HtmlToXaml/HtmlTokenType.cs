﻿namespace MedLaunch.Classes.HtmlToXaml
{
    /// <summary>
    ///     types of lexical tokens for html-to-xaml converter
    /// </summary>
    internal enum HtmlTokenType
    {
        OpeningTagStart,
        ClosingTagStart,
        TagEnd,
        EmptyTagEnd,
        EqualSign,
        Name,
        Atom, // any attribute value not in quotes
        Text, //text content when accepting text
        Comment,
        Eof
    }
}
