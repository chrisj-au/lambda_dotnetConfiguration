
namespace lambda_dotnetConfiguration {
    public class AppConfig {
        public string appParam1 { get; set; }
        public SomeClass childParam { get; set; }
    }
    public class SomeClass {
        public string[] arrValues { get; set; }
    }
}