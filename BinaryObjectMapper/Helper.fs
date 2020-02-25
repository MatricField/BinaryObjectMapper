namespace BinaryObjectMapper.Helper

type internal OptionBuilder() =
    member inline _.Bind(m, f) = 
        Option.bind f m

    member inline _.Return(v) = 
        Some v

    member inline _.ReturnFrom(m) = 
        m

    member inline _.Yield(v) = 
        Some v

    member inline _.YieldFrom(m) = 
        m

    member inline _.Zero() = 
        None

module internal Option =
    let maybe = OptionBuilder()