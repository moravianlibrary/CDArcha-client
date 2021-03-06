﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CDArcha_klient
{
    /// <summary>
    /// Represents parameters for upload to obalkyknih.cz
    /// </summary>
    public class UploadParameters
    {
        /// <summary>
        /// URL where will be files uploaded
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Stream containing cover image
        /// </summary>
        public string CoverFilePath { get; set; }

        /// <summary>
        /// Stream containing tiff with toc images
        /// </summary>
        public List<string> TocFilePaths { get; set; }

        /// <summary>
        /// Stream containing tiff with auth images
        /// </summary>
        public List<string> AuthFilePaths { get; set; }

        /// <summary>
        /// Stream containing meta informations
        /// </summary>
        public string MetaXml { get; set; }

        /// <summary>
        /// Stream containing MarcXml
        /// </summary>
        public string MarcXml { get; set; }

        /// <summary>
        /// Stream MODS record
        /// </summary>
        public string MetaMods { get; set; }

        /// <summary>
        /// Collection of string parameters (Title, Author, Year, Identifiers, Login, Password)
        /// </summary>
        public NameValueCollection Nvc { get; set; }
    }
}
