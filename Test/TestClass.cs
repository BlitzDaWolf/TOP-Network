namespace Test
{
    public class subsubclass
    {
        public int MyProperty { get; set; }
    }

    public class SubClass
    {
        public int sub { get; set; }
        public subsubclass[] Testing { get; set; }
    }

    public class TestClass
    {
        public int test { get; set; }
        public string Hallo { get; set; }

        public SubClass sc { get; set; }


        public int[] ArrayTest { get; set; }
    }
}
