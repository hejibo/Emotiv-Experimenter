﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCAEmotiv.GUI.Animation;
using System.Windows.Forms;

namespace MCAEmotiv.GUI.KRMonitor
{
    class VocabView : MCAEmotiv.GUI.Animation.View
    {
        public VocabView(string testStimulus, string correctAns, int displayTimeMillis, int delayTimeMillis, out IViewResult result)
            : base()
        {
            TextView test = new TextView(testStimulus, displayTimeMillis); //-1 is infinite time
            string[] answers = new string[1];
            //Currently the only option is the correct answer
            //To do: Randomly select a subset of answers OR do free response
            answers[0] = correctAns;
            ChoiceView choice = new ChoiceView(answers);
            var timer = this.RegisterDisposable(new Timer() { Interval = delayTimeMillis, Enabled = false });
            var rows = GUIUtils.CreateTable(new[] { .5, .5 }, Direction.Vertical);
            var testPanel = new Panel { Dock = DockStyle.Fill };
            var choicePanel = new Panel { Dock = DockStyle.Fill, Enabled = false };
            rows.Controls.Add(testPanel, 0, 0);
            timer.Tick += (sender, args) =>
            {
                choicePanel.Enabled = true;
                rows.Controls.Add(choicePanel, 0, 1);
                timer.Enabled = false;
            };
            this.DoOnDeploy(c =>
            {
                c.Controls.Add(rows);
                this.DeploySubView(test, testPanel, causesOwnerToFinish: false);
                this.DeploySubView(choice, choicePanel, causesOwnerToFinish: true);
                timer.Enabled = true;
            });
            this.DoOnFinishing(() =>
                {
                    var answer = choice.Result.HasValue ? choice.Result.Value : null;
                    this.SetResult(answer == correctAns);
                });
            result = this.Result;
        }
    }
}