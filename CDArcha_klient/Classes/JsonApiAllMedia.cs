using System;
using System.Collections.Generic;


namespace CDArcha_klient
{

    /// <summary>
    /// Api data structere of response to get all child archives and media of bib. record
    /// </summary>
    public class JsonApiAllMedia
    {
        public string id { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string mediaNo { get; set; }
        public string dtLastUpdate { get; set; }
        public string text { get; set; }
    }

}
