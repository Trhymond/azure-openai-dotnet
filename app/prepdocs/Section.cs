﻿
internal readonly record struct Section(
    string Id,
    string Content,
    string SourcePage,
    string SourceFile,
    string? Category);
