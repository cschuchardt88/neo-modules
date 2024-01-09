// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Plugins.RestServer is free software distributed under the MIT software license,
// see the accompanying file LICENSE in the main directory of the
// project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Integer = Neo.VM.Types.Integer;

namespace Neo.Plugins.RestServer.Newtonsoft.Json
{
    public class VmIntegerJsonConverter : JsonConverter<Integer>
    {
        public override Integer ReadJson(JsonReader reader, Type objectType, Integer? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var t = JToken.ReadFrom(reader);
            if (RestServerUtility.StackItemFromJToken(t) is Integer i) return i;

            throw new FormatException();
        }

        public override void WriteJson(JsonWriter writer, Integer? value, JsonSerializer serializer)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            var t = RestServerUtility.StackItemToJToken(value, null, serializer);
            t.WriteTo(writer);
        }
    }
}
