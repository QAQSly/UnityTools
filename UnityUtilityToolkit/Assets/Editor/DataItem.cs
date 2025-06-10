using System;
using System.Collections.Generic;
namespace Sly
{	[System.Serializable]
	public class DataItem
	{
		//道具id
		public int id;

		//道具名称
		public String name;

		//道具类型
		public List<int> types;

		//道具文本（描述、信息、解锁方法）
		public List<String> texts;
	}
	
}