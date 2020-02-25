namespace BinaryObjectMapper

open System
open System.IO

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Struct)>]
type MappableTypeAttribute() =
    inherit Attribute()

type IMappableType =
    abstract member Serialize: BinaryWriter -> unit
    abstract member Deserialize: BinaryReader -> unit