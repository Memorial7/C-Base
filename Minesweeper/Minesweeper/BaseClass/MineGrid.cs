using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//7.3
namespace Minesweeper.BaseClass
{
    class MineGrid
    {
        private int row;
        private int col;
        private int state;
        private int oldstate;
        private bool isMine;

        
       //读写属性，类实例的行
        public int Row
        {
            get { return row; }
            set { row = value; }
        }

        //只读属性，类实例的列
        public int Col
        {
            get { return col; }
            set { col = value; }
        }

        //读写属性，类实例的显示状态
        public int State
        {
            get { return state; }
            set { state = value; }
        }

        //读写属性,类实例原来的显示状态
        public int OldState
        {
            get { return oldstate; }
            set { oldstate = value; }
        }
        //读写属性，类实例是否为雷
        public bool IsMine
        {
            get { return isMine; }
            set { isMine = value; }
        }
    }
}
