using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Vici.Core.Test
{
    [TestFixture]
    public class ObjectConverterTest
    {
        private enum _TestByteEnum : byte
        {
            Zero = 0,One = 1,Two = 2,Big = Byte.MaxValue
        }

        private enum _TestIntEnum : int
        {
            Zero = 0,One = 1,Two = 2,Big = Int32.MaxValue
        }

        private enum _TestLongEnum : long
        {
            Zero = 0,One = 1,Two = 2,Big = Int64.MaxValue
        }

        private void RunTests<T,TSource>(Dictionary<TSource,T> items)
        {
            foreach (var item in items)
                Assert.AreEqual(item.Value, item.Key.Convert<T>());
        }

        [Test]
        public void NullToInt()
        {
            Assert.AreEqual(0, ((object)null).Convert<int>());
            Assert.AreEqual(null, ((object)null).Convert<int?>());
        }

        [Test]
        public void StringsToInt()
        {
            RunTests(new Dictionary<string, int> {{"111", 111}, {"", 0}, {"A", 0}});
            RunTests(new Dictionary<string, int?> {{"111", 111}, {"", null}, {"A", null}});
        }

        [Test]
        public void DoublesToInt()
        {
            RunTests(new Dictionary<double, int> { { 111.0, 111 }, { 111.4, 111 }, { 111.5, 112 }, { 111.6, 112 } });
            RunTests(new Dictionary<double, int?> { { 111.0, 111 }, { 111.4, 111 }, { 111.5, 112 }, { 111.6, 112 } });
        }

        [Test]
        public void FloatsToInt()
        {
            RunTests(new Dictionary<float, int> { { 111.0f, 111 }, { 111.4f, 111 }, { 111.5f, 112 }, { 111.6f, 112 } });
            RunTests(new Dictionary<float, int?> { { 111.0f, 111 }, { 111.4f, 111 }, { 111.5f, 112 }, { 111.6f, 112 } });
        }

        [Test]
        public void EnumToInt()
        {
            RunTests(new Dictionary<Enum, int> { { _TestByteEnum.Zero, 0 }, { _TestByteEnum.One, 1 }, { _TestByteEnum.Two, 2}, {_TestByteEnum.Big, Byte.MaxValue} });
            RunTests(new Dictionary<Enum, int> { { _TestIntEnum.Zero, 0 }, { _TestIntEnum.One, 1 }, { _TestIntEnum.Two, 2 }, { _TestIntEnum.Big, Int32.MaxValue } });
            RunTests(new Dictionary<Enum, int> { { _TestLongEnum.Zero, 0 }, { _TestLongEnum.One, 1 }, { _TestLongEnum.Two, 2 }, { _TestLongEnum.Big, 0 } });
            RunTests(new Dictionary<Enum, int?> { { _TestByteEnum.Zero, 0 }, { _TestByteEnum.One, 1 }, { _TestByteEnum.Two, 2 }, { _TestByteEnum.Big, Byte.MaxValue } });
            RunTests(new Dictionary<Enum, int?> { { _TestIntEnum.Zero, 0 }, { _TestIntEnum.One, 1 }, { _TestIntEnum.Two, 2 }, { _TestIntEnum.Big, Int32.MaxValue } });
            RunTests(new Dictionary<Enum, int?> { { _TestLongEnum.Zero, 0 }, { _TestLongEnum.One, 1 }, { _TestLongEnum.Two, 2 }, { _TestLongEnum.Big, null } });
        }

        [Test]
        public void JunkToInt()
        {
            RunTests(new Dictionary<object, int> { { new{}, 0 }, { DateTime.Now, 0 }});
            RunTests(new Dictionary<object, int?> { { new { }, null }, { DateTime.Now, null } });
        }

        [Test]
        public void NullToByte()
        {
            Assert.AreEqual(0, ((object)null).Convert<byte>());
            Assert.AreEqual(null, ((object)null).Convert<byte?>());
        }

        [Test]
        public void StringsToByte()
        {
            RunTests(new Dictionary<string, byte> { { "111", 111 }, { "", 0 }, { "A", 0 } });
            RunTests(new Dictionary<string, byte?> { { "111", 111 }, { "", null }, { "A", null } });
        }

        [Test]
        public void EnumToByte()
        {
            RunTests(new Dictionary<Enum, byte> { { _TestByteEnum.Zero, 0 }, { _TestByteEnum.One, 1 }, { _TestByteEnum.Two, 2 }, { _TestByteEnum.Big, Byte.MaxValue } });
            RunTests(new Dictionary<Enum, byte> { { _TestIntEnum.Zero, 0 }, { _TestIntEnum.One, 1 }, { _TestIntEnum.Two, 2 }, { _TestIntEnum.Big, 0 } });
            RunTests(new Dictionary<Enum, byte> { { _TestLongEnum.Zero, 0 }, { _TestLongEnum.One, 1 }, { _TestLongEnum.Two, 2 }, { _TestLongEnum.Big, 0 } });
            RunTests(new Dictionary<Enum, byte?> { { _TestByteEnum.Zero, 0 }, { _TestByteEnum.One, 1 }, { _TestByteEnum.Two, 2 }, { _TestByteEnum.Big, Byte.MaxValue } });
            RunTests(new Dictionary<Enum, byte?> { { _TestIntEnum.Zero, 0 }, { _TestIntEnum.One, 1 }, { _TestIntEnum.Two, 2 }, { _TestIntEnum.Big, null } });
            RunTests(new Dictionary<Enum, byte?> { { _TestLongEnum.Zero, 0 }, { _TestLongEnum.One, 1 }, { _TestLongEnum.Two, 2 }, { _TestLongEnum.Big, null } });
        }

        [Test]
        public void NullToLong()
        {
            Assert.AreEqual(0, ((object)null).Convert<long>());
            Assert.AreEqual(null, ((object)null).Convert<long?>());
        }

        [Test]
        public void StringsToLong()
        {
            RunTests(new Dictionary<string, long> { { "111", 111 }, { "", 0 }, { "A", 0 } });
            RunTests(new Dictionary<string, long?> { { "111", 111 }, { "", null }, { "A", null } });
        }

        [Test]
        public void EnumToLong()
        {
            RunTests(new Dictionary<Enum, long> { { _TestByteEnum.Zero, 0 }, { _TestByteEnum.One, 1 }, { _TestByteEnum.Two, 2 }, { _TestByteEnum.Big, Byte.MaxValue } });
            RunTests(new Dictionary<Enum, long> { { _TestIntEnum.Zero, 0 }, { _TestIntEnum.One, 1 }, { _TestIntEnum.Two, 2 }, { _TestIntEnum.Big, Int32.MaxValue } });
            RunTests(new Dictionary<Enum, long> { { _TestLongEnum.Zero, 0 }, { _TestLongEnum.One, 1 }, { _TestLongEnum.Two, 2 }, { _TestLongEnum.Big, Int64.MaxValue } });
            RunTests(new Dictionary<Enum, long?> { { _TestByteEnum.Zero, 0 }, { _TestByteEnum.One, 1 }, { _TestByteEnum.Two, 2 }, { _TestByteEnum.Big, Byte.MaxValue } });
            RunTests(new Dictionary<Enum, long?> { { _TestIntEnum.Zero, 0 }, { _TestIntEnum.One, 1 }, { _TestIntEnum.Two, 2 }, { _TestIntEnum.Big, Int32.MaxValue } });
            RunTests(new Dictionary<Enum, long?> { { _TestLongEnum.Zero, 0 }, { _TestLongEnum.One, 1 }, { _TestLongEnum.Two, 2 }, { _TestLongEnum.Big, Int64.MaxValue } });
        }

        [Test]
        public void IntsToString()
        {
            RunTests(new Dictionary<object, string> { { 111, "111" } });
        }

        [Test]
        public void NullToString()
        {
            Assert.AreEqual(null, ((object)null).Convert<string>());
        }

        [Test]
        public void IntToEnum()
        {
            RunTests(new Dictionary<int, _TestByteEnum> { { 0,_TestByteEnum.Zero }, { 1,_TestByteEnum.One }, { 2,_TestByteEnum.Two }, { 999, _TestByteEnum.Zero } });
            RunTests(new Dictionary<int, _TestByteEnum?> { { 0, _TestByteEnum.Zero }, { 1, _TestByteEnum.One }, { 2, _TestByteEnum.Two }, { 999, null } });
            RunTests(new Dictionary<int, _TestIntEnum> { { 0, _TestIntEnum.Zero }, { 1, _TestIntEnum.One }, { 2, _TestIntEnum.Two }, { 999, _TestIntEnum.Zero } });
            RunTests(new Dictionary<int, _TestIntEnum?> { { 0, _TestIntEnum.Zero }, { 1, _TestIntEnum.One }, { 2, _TestIntEnum.Two }, { 999, null } });
            RunTests(new Dictionary<int, _TestLongEnum> { { 0, _TestLongEnum.Zero }, { 1, _TestLongEnum.One }, { 2, _TestLongEnum.Two }, { 999, _TestLongEnum.Zero } });
            RunTests(new Dictionary<int, _TestLongEnum?> { { 0, _TestLongEnum.Zero }, { 1, _TestLongEnum.One }, { 2, _TestLongEnum.Two }, { 999, null } });
        }

        [Test]
        public void DoubleToEnum()
        {
            RunTests(new Dictionary<double, _TestByteEnum> { { 0, _TestByteEnum.Zero }, { 1, _TestByteEnum.One }, { 2, _TestByteEnum.Two }, { 999, _TestByteEnum.Zero } });
            RunTests(new Dictionary<double, _TestByteEnum?> { { 0, _TestByteEnum.Zero }, { 1, _TestByteEnum.One }, { 2, _TestByteEnum.Two }, { 999, null } });
            RunTests(new Dictionary<double, _TestIntEnum> { { 0, _TestIntEnum.Zero }, { 1, _TestIntEnum.One }, { 2, _TestIntEnum.Two }, { 999, _TestIntEnum.Zero } });
            RunTests(new Dictionary<double, _TestIntEnum?> { { 0, _TestIntEnum.Zero }, { 1, _TestIntEnum.One }, { 2, _TestIntEnum.Two }, { 999, null } });
            RunTests(new Dictionary<double, _TestLongEnum> { { 0, _TestLongEnum.Zero }, { 1, _TestLongEnum.One }, { 2, _TestLongEnum.Two }, { 999, _TestLongEnum.Zero } });
            RunTests(new Dictionary<double, _TestLongEnum?> { { 0, _TestLongEnum.Zero }, { 1, _TestLongEnum.One }, { 2, _TestLongEnum.Two }, { 999, null } });
        }

        [Test]
        public void StringToEnum()
        {
            RunTests(new Dictionary<string, _TestByteEnum> { { "0", _TestByteEnum.Zero }, { "1", _TestByteEnum.One }, { "2", _TestByteEnum.Two }, { "999", _TestByteEnum.Zero }, { "Zero", _TestByteEnum.Zero }, { "One", _TestByteEnum.One }, { "Two", _TestByteEnum.Two }, { "", _TestByteEnum.Zero }, { "X", _TestByteEnum.Zero } });
            RunTests(new Dictionary<string, _TestByteEnum?> { { "0", _TestByteEnum.Zero }, { "1", _TestByteEnum.One }, { "2", _TestByteEnum.Two }, { "999", null }, { "Zero", _TestByteEnum.Zero }, { "One", _TestByteEnum.One }, { "Two", _TestByteEnum.Two }, { "", null }, { "X", null } });
            RunTests(new Dictionary<string, _TestIntEnum> { { "0", _TestIntEnum.Zero }, { "1", _TestIntEnum.One }, { "2", _TestIntEnum.Two }, { "999", _TestIntEnum.Zero }, { "Zero", _TestIntEnum.Zero }, { "One", _TestIntEnum.One }, { "Two", _TestIntEnum.Two }, { "", _TestIntEnum.Zero }, { "X", _TestIntEnum.Zero } });
            RunTests(new Dictionary<string, _TestIntEnum?> { { "0", _TestIntEnum.Zero }, { "1", _TestIntEnum.One }, { "2", _TestIntEnum.Two }, { "999", null }, { "Zero", _TestIntEnum.Zero }, { "One", _TestIntEnum.One }, { "Two", _TestIntEnum.Two }, { "", null }, { "X", null } });
            RunTests(new Dictionary<string, _TestLongEnum> { { "0", _TestLongEnum.Zero }, { "1", _TestLongEnum.One }, { "2", _TestLongEnum.Two }, { "999", _TestLongEnum.Zero }, { "Zero", _TestLongEnum.Zero }, { "One", _TestLongEnum.One }, { "Two", _TestLongEnum.Two }, { "", _TestLongEnum.Zero }, { "X", _TestLongEnum.Zero } });
            RunTests(new Dictionary<string, _TestLongEnum?> { { "0", _TestLongEnum.Zero }, { "1", _TestLongEnum.One }, { "2", _TestLongEnum.Two }, { "999", null }, { "Zero", _TestLongEnum.Zero }, { "One", _TestLongEnum.One }, { "Two", _TestLongEnum.Two }, { "", null }, { "X", null } });
        }

        [Test]
        public void IntToDateTime()
        {
            RunTests(new Dictionary<int,DateTime>{{100, new DateTime(1970,1,1).AddSeconds(100)}});
        }

        [Test]
        public void StringToDateTime()
        {
            RunTests(new Dictionary<string, DateTime> { { "100", new DateTime(1970, 1, 1).AddSeconds(100) }, { "2014-12-31", new DateTime(2014, 12, 31) }, { "2014-12-31 22:23:24", new DateTime(2014, 12, 31, 22, 23, 24) }, { "2014-12-31T22:23:24", new DateTime(2014, 12, 31, 22, 23, 24) }, { "", DateTime.MinValue }, { "X", DateTime.MinValue } });
            RunTests(new Dictionary<string, DateTime?> { { "100", new DateTime(1970, 1, 1).AddSeconds(100) }, { "2014-12-31", new DateTime(2014, 12, 31) }, { "2014-12-31 22:23:24", new DateTime(2014, 12, 31, 22, 23, 24) }, { "2014-12-31T22:23:24", new DateTime(2014, 12, 31, 22, 23, 24) }, { "", null }, { "X", null } });
        }

        [Test]
        public void NullToDateTime()
        {
            Assert.AreEqual(DateTime.MinValue, ((object)null).Convert<DateTime>());
            Assert.AreEqual(null, ((object)null).Convert<DateTime?>());
        }

    }
}