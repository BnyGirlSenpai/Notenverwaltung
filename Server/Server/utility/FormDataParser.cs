namespace WebServer.Server.utility
{
    internal class FormDataParser
    {
        public Dictionary<string, string> FormData { get; private set; }

        private FormDataParser(Dictionary<string, string> formData)
        {
            FormData = formData;
        }

        public static FormDataParser Parse(string body)
        {
            var formData = new Dictionary<string, string>();
            var keyValuePairs = body.Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in keyValuePairs)
            {
                var keyValue = pair.Split('=', 2);
                if (keyValue.Length == 2)
                {
                    formData[Uri.UnescapeDataString(keyValue[0])] = Uri.UnescapeDataString(keyValue[1]);
                }
            }

            return new FormDataParser(formData);
        }

        public string GetValue(string key)
        {
            return FormData.TryGetValue(key, out var value) ? value : null;
        }

        public bool ContainsKey(string key)
        {
            return FormData.ContainsKey(key);
        }
    }
}
