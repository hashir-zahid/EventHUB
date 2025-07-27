using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpPage
{
    public class FaqItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public static class FaqProvider
    {
        public static List<FaqItem> GetFaqs()
        {
            return new List<FaqItem>
            {
                 new FaqItem
                {
                    Question = "Where can I see the list of upcoming events?",
                    Answer = "Visit the Events section in the sidebar. All upcoming events are listed there with dates, descriptions, and registration options."
                },
                 new FaqItem
                {
                    Question = "How do attendees register?",
                    Answer = "To register, go to the \"Events\" section, select the event you're interested in, and click the \"Register\" button."
                },
                 new FaqItem
                {
                    Question = "What if I made a mistake during registration?",
                    Answer = "Go to \"My Tickets\", Enter you Id, and click \"Filter\" and check you ticket to confirm your registeration."
                },
                new FaqItem
                {
                    Question = " Are videos available for all past events?",
                    Answer = "Not all events have video coverage. If available, you can find it by clicking on the Media Library in the sidebar under the ‘Explore’ section."
                },
            };
        }
    }
}
