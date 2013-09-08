.NET-Class-Change-Logger
========================

Library to compare two instances of a class and return a list of changes between them

This library allows you to compare two instances of a class object and then return a list of differences between them. It will allow to to deep compare classes.

To create comparison classes I would recommend using https://github.com/Burtsev-Alexey/net-object-deep-copy to create a clone of your original class, apply the changes to the properties of the original instance and then Audit it using this library.
