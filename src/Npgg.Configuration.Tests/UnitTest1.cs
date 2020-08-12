using System;
using Xunit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Npgg.Configuration;

namespace Npgg.Tests
{
	public class CustomConverterTests
	{

		[Fact]
		public void CustomConverterTest()
		{
			var csvString = @"Key,Vector3
1,""(1,2,3)""
";

			CustomConverter<Vector3>.RegistConverter<Vector3Converter>();

			CsvLoader loader = new CsvLoader();

			var list = loader.Load<Vector3Row>(csvString);

			Assert.Single(list);

			var item = list.FirstOrDefault();
			Assert.NotNull(item);
			Assert.Equal(1, item.Key);
			Assert.Equal(1, item.Vector3.X);
			Assert.Equal(2, item.Vector3.Y);
			Assert.Equal(3, item.Vector3.Z);
		}

		
		class Vector3Row
		{
			public int Key { get; set; }
		
			public Vector3 Vector3 { get; set; }
		}

		class Vector3
		{
			public float X, Y, Z;
		}

		class Vector3Converter : CustomConverter<Vector3>
		{
			public override Vector3 Convert(string value) //input (1,2,3)
			{
				var splited = value.Split(',')
					.Select(d => d.Trim('(', ')')) // array["1", "2", "3"]
					.Select(d => float.Parse(d)).ToArray(); // array[1, 2, 3]

				return new Vector3() { X = splited[0], Y = splited[1], Z = splited[2] };
			
			}
		}
	}

    public class UnitTest1
    {

        CsvLoader loader = new CsvLoader();
        

        string MakeRow (params object[] values)=> string.Join(',', values);

        string MakeQuotationMarkedRow(params object[] values) => string.Join(',', values.Select(value=> $"\"{value}\""));

		[Fact]
		public void CustomConverterTest()
		{

		}

        [Fact]
        public void TestValueCreateTest1()
        {
            var generated = MakeQuotationMarkedRow(1, 2);

            Assert.Equal("\"1\",\"2\"", generated);
        }
        [Fact]
        public void TestValueCreateTest2()
        {
            var generated = MakeRow(1, 2, 3,4);

            Assert.Equal("1,2,3,4", generated);
        }

        [Fact]
        public void CsvLoadTest()
        {
            string csv =
@"Key,Value
0,Value0
1,Value1
";

            var loaded = loader.Load<CsvSample>(csv);

            Assert.Equal(2, loaded.Count);

            for (int i = 0; i < 2; i++)
            {
                var item = loaded[i];

                Assert.Equal(i, item.Key);
                Assert.Equal("Value" + i, item.Value);
            }
        }


        [Fact]
        public void RowTest()
        {
            string csv =
@"Key,Value
1,Value1
#2,Value2
2,Value2
";

            var loaded = loader.Load<CsvSample>(csv);

            Assert.Equal(2, loaded.Count);

            {
                var item = loaded.First();

                Assert.Equal(1, item.Key);
                Assert.Equal("Value1", item.Value);
            }
            {
                var item = loaded.Last();

                Assert.Equal(2, item.Key);
                Assert.Equal("Value2", item.Value);
            }
        }

        [Fact]
        public void ColumnTest()//d?諛ㅺ???
        {
            string csv =
@"Key,#Value
0,Value0
1,Value1
";

            var loaded = loader.Load<CsvSample>(csv);

            Assert.Equal(2, loaded.Count);

            for (int i = 0; i < 2; i++)
            {
                var item = loaded[i];

                Assert.Equal(i, item.Key);
                Assert.Null(item.Value);
            }
        }

        public class CsvSample
        {
            public int Key { get; set; }
			public string Value;

			[ConfigColumn("camel_case")]
			public string CamelCase;
		}


		[Fact]
		public void AttributesTest()
		{

			var type = typeof(CsvSample);

			var members = type.GetMembers();
				//BindingFlags.GetField| BindingFlags.SetField| BindingFlags.SetProperty| BindingFlags.GetProperty| BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			var ss = members.Select(mem => mem.Name).ToArray();

			foreach (var mem in members)
			{
				var xx = mem.GetCustomAttribute<ConfigColumnAttribute>()?.ColumnName ?? mem.Name;
			}


			//


			string csv =
@"Key,#Value,camel_case
0,Value0,ok
1,Value1,ok
";

			var loaded = loader.Load<CsvSample>(csv);

			Assert.Equal(2, loaded.Count);

			foreach(var item in loaded)
			{
				Assert.Equal("ok", item.CamelCase);
			}
				

		}
    }

    



    
}
