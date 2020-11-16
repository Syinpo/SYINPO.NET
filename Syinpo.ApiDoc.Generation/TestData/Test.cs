using AutoBogus;

namespace Syinpo.ApiDoc.Generation.TestData
{
    class Test
    {
        /// <summary>
        /// https://github.com/bchavez/Bogus
        /// http://jackhiston.com/2017/10/1/how-to-create-bogus-data-in-c/
        ///
        /// https://github.com/nickdodd79/AutoBogus
        /// </summary>
        public void TestData()
        {
            var a = AutoFaker.Generate<A>();


        }

        public class A{
        }

    }
}
