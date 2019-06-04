using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistCreator
{
    class Song
    {
        private int _seconds;

        public int seconds
        {
            get { return _seconds; }
            set { _seconds = value; }
        }

        private string _name;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _path;

        public string path
        {
            get { return _path; }
            set { _path = value; }
        }

        public Song()
        {

        }

        public Song(int seconds, string name, string path)
        {
            _seconds = seconds;
            _name = name;
            _path = path;
        }

        
    }
}
