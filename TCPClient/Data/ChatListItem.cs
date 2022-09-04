using System;
using System.ComponentModel.DataAnnotations;

namespace TCPClient.Data
{
    internal class ChatListItem
    {
        public int Id { get; set; }
        public byte From { get; set; }
        public byte To { get; set; }
        public string Message { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime DateTime { get; set; }
    }
}
