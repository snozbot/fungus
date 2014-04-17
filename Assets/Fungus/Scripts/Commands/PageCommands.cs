using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	/**
	 *  Command classes have their own namespace to prevent them popping up in code completion.
	 */
	namespace Command
	{
		/**
		 * Sets the display rect for the active Page using a PageBounds object.
		 */
		public class SetPageBounds : CommandQueue.Command
		{
			PageBounds pageBounds;
			Page.Layout pageLayout;
			
			public SetPageBounds(PageBounds _pageBounds, Page.Layout _pageLayout)
			{
				pageBounds = _pageBounds;
				pageLayout = _pageLayout;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				if (pageBounds != null)
				{
					pageBounds.UpdatePageRect();
					Game.GetInstance().activePage.layout = pageLayout;
				}
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/**
		 * Sets the display rect for the active Page using normalized screen space coords.
		 */
		public class SetPageRect : CommandQueue.Command
		{
			float x1;
			float y1;
			float x2;
			float y2;
			Page.Layout layout;
			
			public SetPageRect(float _x1, float _y1, float _x2, float _y2, Page.Layout _layout)
			{
				x1 = _x1;
				y1 = _y1;
				x2 = _x2;
				y2 = _y2;
				layout = _layout;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Page page = Game.GetInstance().activePage;
				page.SetPageRect(x1, y1, x2, y2);
				page.layout = layout;
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/**
		 * Sets the currently active Page Style for rendering Pages.
		 */
		public class SetPageStyle : CommandQueue.Command
		{
			PageStyle pageStyle;
			
			public SetPageStyle(PageStyle _pageStyle)
			{
				pageStyle = _pageStyle;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game.GetInstance().activePageStyle = pageStyle;
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/**
		 * Sets the header text displayed at the top of the active page.
		 */
		public class SetHeader : CommandQueue.Command
		{
			string titleText;
			
			public SetHeader(string _titleText)
			{
				titleText = _titleText;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Page page = Game.GetInstance().activePage;
				if (page == null)
				{
					Debug.LogError("Active page must not be null");
				}
				else
				{
					page.SetHeader(titleText);
				}
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/**
		 * Sets the footer text displayed at the top of the active page.
		 */
		public class SetFooter : CommandQueue.Command
		{
			string titleText;
			
			public SetFooter(string _titleText)
			{
				titleText = _titleText;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Page page = Game.GetInstance().activePage;
				if (page == null)
				{
					Debug.LogError("Active page must not be null");
				}
				else
				{
					page.SetFooter(titleText);
				}
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/** 
		 * Writes story text to the currently active page.
		 * A 'continue' icon is displayed when the text has fully appeared.
		 */
		public class Say : CommandQueue.Command
		{
			string storyText;
			
			public Say(string _storyText)
			{
				storyText = _storyText;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Page page = Game.GetInstance().activePage;
				if (page == null)
				{
					Debug.LogError("Active page must not be null");
				}
				else
				{
					page.Say(storyText, onComplete);
				}
			}
		}
		
		/** 
		 * Adds an option button to the current list of options.
		 * Use the Choose command to display added options.
		 */
		public class AddOption : CommandQueue.Command
		{
			string optionText;
			Action optionAction;
			
			public AddOption(string _optionText, Action _optionAction)
			{
				optionText = _optionText;
				optionAction = _optionAction;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Page page = Game.GetInstance().activePage;
				if (page == null)
				{
					Debug.LogError("Active page must not be null");
				}
				else
				{
					page.AddOption(optionText, optionAction);
				}
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/**
		 * Displays all previously added options.
		 */
		public class Choose : CommandQueue.Command
		{
			string chooseText;
			
			public Choose(string _chooseText)
			{
				chooseText = _chooseText;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Page page = Game.GetInstance().activePage;
				if (page == null)
				{
					Debug.LogError("Active page must not be null");
				}
				else
				{
					page.Choose(chooseText);
				}
				// Choose always clears commandQueue, so no need to call onComplete()
			}		
		}
	}
}
