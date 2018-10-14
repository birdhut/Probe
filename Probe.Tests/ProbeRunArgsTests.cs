namespace Probe.Tests
{
    using System;
    using Xunit;

    public class ProbeRunArgsTests
    {
        private readonly ProbeRunArgs args;
        public ProbeRunArgsTests()
        {
            args = new ProbeRunArgs();
        }

        [Fact]
        public void Can_parse_string_argument()
        {
            var key = "string-test";
            var value = "This is a string";
            args.Add(key, value);

            var result = args.ParseStringArg(key);

            Assert.Equal(value, result);
        }

        [Fact]
        public void Can_parse_long_number_argument()
        {
            var key = "long-test";
            var value = 100;
            args.Add(key, value.ToString());

            var result = args.ParseLongNumberArg(key);

            Assert.Equal(value, result);
        }

        [Fact]
        public void Can_parse_double_number_argument()
        {
            var key = "double-test";
            var value = 100.01;
            args.Add(key, value.ToString());

            var result = args.ParseDoubleNumberArg(key);

            Assert.Equal(value, result);
        }

        [Fact]
        public void Can_parse_date_argument()
        {
            var key = "date-test";
            var value = DateTime.Now.Date;
            args.Add(key, value.ToString("yyyy-MM-dd"));

            var result = args.ParseDateArg(key);

            Assert.Equal(value, result);
        }

        [Fact]
        public void Can_parse_datetime_argument()
        {
            var key = "datetime-test";
            var value = DateTime.Parse(DateTime.UtcNow.ToString("s"));
            args.Add(key, value.ToString("s"));

            var result = args.ParseDateTimeArg(key);

            Assert.Equal(value, result);
        }

        [Fact]
        public void Unparsable_value_throws_probe_arg_parse_exception()
        {
            var key1 = "unparse-date";
            var key2 = "unparse-long";
            var key3 = "unparse-double";
            var key4 = "unparse-datetime";
            args.Add(key1, "Not a date");
            args.Add(key2, "Not a long");
            args.Add(key3, "Not a double");
            args.Add(key4, "Not a datetime");

            Assert.Throws<ProbeArgParseException>(() => args.ParseDateArg(key1));
            Assert.Throws<ProbeArgParseException>(() => args.ParseLongNumberArg(key2));
            Assert.Throws<ProbeArgParseException>(() => args.ParseDoubleNumberArg(key3));
            Assert.Throws<ProbeArgParseException>(() => args.ParseDateTimeArg(key4));
        }
    }
}
