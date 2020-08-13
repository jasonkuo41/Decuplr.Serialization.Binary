









This file includes a list of attributes 

## Primitive Annotations

Primitive annotations are those treated as "first class" annotation by the library, 
meaning that it does not expand or reinterprete to other meaning or annotation.
The library directly deals with the notation. 

### Type Annontation

This annotation is mainly applied on to class and struct

#### [BinaryFormat]

Describes a type that should be read by the source generator and generate a parser for the library to utilize it.

#### [BinaryParser]

Notates a type to become a parser provider of a certain type.


### Member Property Annotation

This annotation is mainly applied on to members of a type.
These attritbutes becomes hard constant past compile time.

#### [Index]

Describes the order of each member when BinaryLayout is explicit

#### [Ignore]

Describes that the member should be ignored when BinaryLayout is sequential

#### [FormatAs]

Formats the current type to a convertible target type when serializing, and convert back when deserialized.

#### [UseNamespace]

Notates a type to use the parser provided by a specific namespace. When applied on an attribute, 
this means wherever this attribute applies the namespace is also applied to the target.

### Member Serialize Annotation

This annotations causes value to mutate when constructing the value.
Evaulation order are like this:

#### [IgnoreIf] [IgnoreIfNot]

Ignores the field or property if conditions match

> If evaluate to ignore, the field will not serialized at all and jumps to next statement

#### [BinaryLength]

Defines a binary length for the field

> Note : This also overrides the result Insufficent Data to Faulted, Data insufficiency is check before hand

#### [BitUnion]

***Type Decision Attribute***

Notates a type or a tuple to become bit length fields

#### [Constant]

*Final Verification Attribute*

Notates a type to be a constant value. This can be used for checking if certain value match.

### Member Misc Annotations

#### [ItemCount]

*Applies to only dynamic length types*

Defines how much item is in this field


## Future Anotations

#### [InlineData]

Reprensents a data that can only be serialized within other data with specific data or interface provided.

Example

```csharp
[InlineData]
public class NestedList<T> {
	public NestedList(NestedInfo info) {
	}
}