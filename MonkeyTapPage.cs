/** [!]. ABOUT
 * "Tap Gestures"
 * by Matthew T. Theroux
 * for Baker College Online CS4010
 * Module 08 Assignment 03 (#17)
 * under the supervision of Joan Zito
 * (c) 2018 March 04 09:00
 --Demonstrates Xamarin's touch "click" events
   with a classic 1980s electronic memory game.
**/
/// [I]. HEAD
///  A] IMPORTS
using System;
using System.Text;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamrin.Forms.core;
using Xamarin.Forms.XAML;
//using Xamarin.*;

namespace tap_gestures
{
    public partial class MonkeyTapPage
    {
        /// B] VARS & consts
        const int sequenceTime = 750; // in milliseconds
        protected const int flashDuration = 250;

        const double offLuminosity = 0.4; // somewhat dimmer 
        const double onLuminosity = 0.75; // much brighter   

        BoxView[] monkeyBoxes; 
        Color[] colors = { Color.Red, Color.Blue, Color.Yellow, Color.Green };
        List<int> sequence = new List<int>();
        int sequenceIndex;
        bool awaitingTaps;
        bool hasGameEnded;
        Random random = new Random();

        /// C] constructor
        public MonkeyTapPage()
        {
            /// read from XAML.
            InitializeComponent();

            monkeyBoxes = new BoxView[] { boxView0, boxView1, boxView2, boxView3 };

            /// 
            InitializeBoxViewColors();

        } // /cxtr

        /// Bring color to the monkeyBoxes.
        void InitializeBoxViewColors()
        {
            for (int index = 0; index < 4; index++)
                monkeyBoxes[index].Color = colors[index].WithLuminosity(offLuminosity);
        }// /meth 'InitializeBoxViewColors'
        
        /// Start a new game.
        protected void startGame_btn_clicked(object sender, EventArgs args) 
        { 
            hasGameEnded = false; 
            startGameButton.IsVisible = false; 

            /// The monkeyBoxes must have color for this to work.
            InitializeBoxViewColors();

            sequence.Clear(); 
            startTheSequence();
        }// /hdlr 'startGame_btn_clicked'


        /// [II]. BODY
        ///  A] Add 1 more random monkeyBox to the sequence
        void startTheSequence()
        {
            sequence.Add(random.Next(4));
            sequenceIndex = 0;

            Device.StartTimer(TimeSpan.FromMilliseconds(sequenceTime), OnTimerTick);
        }// /meth 'startTheSequence'

        /// 
        bool onTimerTick()
        {
            if (hasGameEnded)  return false;

            /// Illuminate the box and advance the sequence queue.
            FlashBoxView(sequence[sequenceIndex]);
            sequenceIndex++;
            awaitingTaps = (sequenceIndex == sequence.Count);
            sequenceIndex = awaitingTaps ? 0 : sequenceIndex;
            return !awaitingTaps;
        } // /hdlr? 'onTimerTick'

        /// *flash* a box with luminosity-- (what, no sounds?).
        protected virtual void FlashBoxView(int index)
        {
            monkeyBoxes[index].Color = colors[index].WithLuminosity(onLuminosity);

            Device.StartTimer(TimeSpan.FromMilliseconds(flashDuration), () => {
                if (hasGameEnded) return false;
                monkeyBoxes[index].Color = colors[index].WithLuminosity(offLuminosity);
                return false;
            }); // /tmr
        }// /meth 'FlashBoxView'

        /// 
        protected void monkeyBox_tapped(object sender, EventArgs args)
        {
            /// 01. Filters
            /// Do nothing if the game has already ended.
            if (hasGameEnded)
                return;

            /// But, if you're no longer listening for taps, it probably s/b.
            if (!awaitingTaps)
            {
                endGame();
                return;
            }// /if '!awaitingTaps'

            /// 02. init
            BoxView tappedBoxView = (BoxView)sender;
            int index = Array.IndexOf(monkeyBoxes, tappedBoxView);

            /// 
            if (index != sequence[sequenceIndex])
            {
                endGame();
                return;
            }// /if (matches the monkey's sequence)

            ///  *flash* that box 
            FlashBoxView(index);
            /// Await the next response of the sequence.
            sequenceIndex++;
            awaitingTaps = (sequenceIndex < sequence.Count);
            if (!awaitingTaps) startTheSequence();
        }// /hdlr 'monkeyBox_tapped'

        /// O] Operations
        public void OnStart() { }
        public void OnSleep() { }
        public void OnResume() { }


        /// [III]. FOOT
        protected virtual void endGame()
        {
            hasGameEnded = true; // :(

            /// Desaturate the boxes.
            for (int index = 0; index < 4; index++)
                monkeyBoxes[index].Color = colors.Gray;

            /// Allow the game to be started, again.
            startGame_btn.Text = "Game Over\nTry Again??";
            startGame_btn.IsVisible = true;
        }// /meth 'endGame'
    }// /cla 'MonkeyTapPage'
}// /ns 'TapGestures'
/// [EoF].