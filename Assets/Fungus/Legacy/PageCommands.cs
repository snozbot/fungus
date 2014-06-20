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
		 * Sets the display rect for the PageController using a Page object.
		 */
		public class SetPage : CommandQueue.Command
		{
			Page page;
			PageController.Layout pageLayout;
			
			public SetPage(Page _page, PageController.Layout _pageLayout)
			{
				page = _page;
				pageLayout = _pageLayout;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				if (page != null)
				{
					page.UpdatePageRect();
					Game.GetInstance().pageController.layout = pageLayout;
				}
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/**
		 * Sets the screen rect for displaying story text using normalized screen space coords.
		 */
		public class SetPageRect : CommandQueue.Command
		{
			PageController.ScreenRect screenRect;
			PageController.Layout layout;
			
			public SetPageRect(PageController.ScreenRect _screenRect, PageController.Layout _layout)
			{
				screenRect = _screenRect;
				layout = _layout;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				PageController page = Game.GetInstance().pageController;
				page.pageRect = PageController.CalcPageRect(screenRect);
				page.layout = layout;
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/**
		 * Sets the active Page Style for rendering story text.
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
				Game.GetInstance().pageController.activePageStyle = pageStyle;
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/**
		 * Sets the header text displayed at the top of the page.
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
				PageController page = Game.GetInstance().pageController;
				page.SetHeader(titleText);

				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/**
		 * Sets the footer text displayed at the bottom of the page.
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
				PageController page = Game.GetInstance().pageController;
				page.SetFooter(titleText);

				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}

		/** 
		 * Writes story text to the page.
		 * A continue icon is displayed when the text has fully appeared.
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
				IDialog sayDialog = Game.GetInstance().GetDialog();
				sayDialog.Say(storyText, onComplete);
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
				IDialog characterDialog = Game.GetInstance().GetDialog();
				characterDialog.AddOption(optionText, optionAction);

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
				IDialog dialog = Game.GetInstance().GetDialog();

				PageController pageController = dialog as PageController;
				if (pageController != null)
				{
					// Legacy support for old Pages system
					pageController.Choose(chooseText);
				}
				else
				{
					// Support for modern IDialog interface
					dialog.Say(chooseText, null);
				}

				// Choose always clears the commandQueue, so there's no need to call onComplete()
			}		
		}
	}
}
