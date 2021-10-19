using System;
using System.ComponentModel.DataAnnotations;

namespace Utils {
    public class BaseColumn {
        [StringLength(20)]
        public string maintainer { get; set; }
        public DateTime createTime { get; set; }
        public DateTime? updateTime { get; set; }
    }
}