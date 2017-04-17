using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSS.UniverseLogic;
using System.Drawing;
using System.Threading;
using System.Diagnostics;


namespace TSS.WinForms
{

    class TextBoxOutputManager
    {
        //int ticksPause, framePause;
        //int ticksPerFrame;
        //float ticksPerSec, framesPerSec;
        //bool onWork = false;
        Universe universe;
        RichTextBox outputRichTextBox;
        Label infoLabel;
        int width, height;
        string status;
        Color border = Color.Black;
        string fieldStr;
        bool disposed;
        LoopWorker loopWorker;
        bool threadMood ;


        public TextBoxOutputManager(Universe universe, RichTextBox outputRichTextBox, Label infoLabel)
        {
            disposed = false;
            threadMood = false;
            width = universe.GetWidth();
            height = universe.GetHeight();
            this.universe = universe;
            this.outputRichTextBox = outputRichTextBox;
            CalcOutputRichTextBox();
            this.infoLabel = infoLabel;
            fieldStr = @"";
            fieldStr += '+';
            for (int i = 0; i < width; i++)
            {
                fieldStr += '-';
            }
            fieldStr += '+';
            status = @"stoped";


            loopWorker= new LoopWorker(this);
            SetTicksPerSecond(2);
            SetFramesPerSecond(2);
        }

        public void Dispose()
        {
            if (loopWorker != null)
            {
                loopWorker.Dispose();
                loopWorker = null;
            }
            universe = null;
            outputRichTextBox = null;
            infoLabel = null;
            status = null;
            fieldStr = null;
            disposed = true;

            
        }
        public bool IsDisposed()
        {
            return disposed;
        }

        //Preworks
        public int CalcOutputRichTextBox()
        {
            Font font;
            int countH = 1;
            outputRichTextBox.Invoke(new Action(() =>
            {
                CalcChars(width + 2, outputRichTextBox.Width, outputRichTextBox.Height, out font, out countH);
                outputRichTextBox.Font = font;
                outputRichTextBox.ScrollBars = RichTextBoxScrollBars.None;
                outputRichTextBox.ReadOnly = true;
                outputRichTextBox.Enabled = false;
            }));
            return countH;
        }
        public static void CalcChars(int count, int width, int height, out Font font, out int countH)
        {
            RichTextBox rtbLoc = new RichTextBox();

            rtbLoc.ScrollBars = RichTextBoxScrollBars.None;
            rtbLoc.Width = width;
            rtbLoc.Height = height;
            font = CalcFont(count, rtbLoc);
            rtbLoc.Font = font;

            countH = rtbLoc.Height / (font.Height + 1);

        }
        static Font CalcFont(int count, RichTextBox rtb)
        {
            float size = 8;

            rtb.Clear();
            for (int i = 0; i < count; i++)
                rtb.AppendText("Q");


            rtb.Font = new Font(@"Courier New", size, FontStyle.Bold);
            int posY = rtb.GetPositionFromCharIndex(0).Y;
            int countDec = count - 1;
            while (rtb.GetPositionFromCharIndex(countDec).Y == posY)
            {
                rtb.Font = new Font(@"Courier New", size, FontStyle.Bold);
                //rtbLoc.Refresh();
                size += (float)0.1;
            }
            size -= (float)(size / 20);

            return new Font(@"Courier New", size, FontStyle.Bold);
        }
        //Preworks

        //Basic rawing functions
        void AppendText(string text)
        {
            outputRichTextBox.Invoke(new Action(() =>
            {
                outputRichTextBox.AppendText(text);
            }));
        }
        void SetColor(int startInd, int length, Color color)
        {
            outputRichTextBox.Invoke(new Action(() =>
            {
                outputRichTextBox.SelectionStart = startInd;
                outputRichTextBox.SelectionLength = length;

                outputRichTextBox.SelectionColor = color;
            }));

        }
        void ClearTextBox()
        {
            outputRichTextBox.Invoke(new Action(() =>
            {
                outputRichTextBox.Clear();
            }));
        }
        //Basic drawing functions

        void DrawPictureDef(int[][] desc)
        {
            SuspendControlUpdate.Suspend(outputRichTextBox);
            ClearTextBox();
            AppendText(OutputStringDef(desc));
            SetAllColorDef(desc);
            SuspendControlUpdate.Resume(outputRichTextBox);

        }
        void DrawPicturePro(DescAndMoveDir[][] descAndMD)
        {
            SuspendControlUpdate.Suspend(outputRichTextBox);
            ClearTextBox();
            AppendText(OutputStringPro(descAndMD));
            SetAllColorPro(descAndMD);
            SuspendControlUpdate.Resume(outputRichTextBox);
        }

        void SetAllColorDef(int[][] desc)
        {
            SetColor(0, fieldStr.Length, border);
            int charNum = fieldStr.Length + 1;
            for (int j = 0; j < height; j++)
            {
                SetColor(charNum, 1, border);
                charNum++;
                for (int i = 0; i < width; i++)
                {
                    int descriptor = desc[i][j];
                    if (descriptor == -1)
                    {
                        SetColor(charNum, 1, Color.Green);
                    }
                    else if (descriptor == -2)
                    {
                        SetColor(charNum, 1, Color.Gray);
                    }
                    else if (descriptor == -3)
                    {
                        SetColor(charNum, 1, Color.DarkOrange);
                    }
                    else if (descriptor >= 100)
                    {
                        SetColor(charNum, 1, Color.FromArgb(descriptor));
                    }
                    charNum++;
                }
                SetColor(charNum, 1, border);
                charNum += 2;


            }
            SetColor(charNum, fieldStr.Length, border);
        }
        void SetAllColorPro(DescAndMoveDir[][] descAndMD)
        {
            SetColor(0, fieldStr.Length, border);

            int charNum = fieldStr.Length + 1;
            for (int j = 0; j < height; j++)
            {
                SetColor(charNum, 1, border);
                charNum++;
                for (int i = 0; i < width; i++)
                {
                    int descriptor = descAndMD[i][j].desc;
                    if (descriptor == -1)
                    {
                        SetColor(charNum, 1, Color.Green);
                    }
                    else if (descriptor == -2)
                    {
                        SetColor(charNum, 1, Color.Gray);
                    }
                    else if (descriptor == -3)
                    {
                        SetColor(charNum, 1, Color.DarkOrange);
                    }
                    else
                    {
                        SetColor(charNum, 1, Color.FromArgb(descriptor));
                    }
                    charNum++;
                }
                SetColor(charNum, 1, border);
                charNum += 2;


            }
            SetColor(charNum, fieldStr.Length, border);
        }

        string OutputStringDef(int[][] desc)
        {
            char cell = 'O';
            char deadCell = '#', food = '*', empty = ' ', poison = '*';
            //int[][] desc = universe.GetAllDescriptors();

            string outputStr = fieldStr + '\n';
            for (int j = 0; j < height; j++)
            {
                outputStr += '|';
                for (int i = 0; i < width; i++)
                {
                    int descriptor = desc[i][j];
                    if (descriptor == 0)
                    {
                        outputStr += empty;
                    }
                    else if (descriptor == -1)
                    {
                        outputStr += food;
                    }
                    else if (descriptor == -2)
                    {
                        outputStr += deadCell;
                    }
                    else if (descriptor == -3)
                    {
                        outputStr += poison;
                    }
                    else
                    {
                        outputStr += cell;
                    }
                }
                outputStr += "|\n";

            }

            outputStr += fieldStr;
            return outputStr;
        }
        string OutputStringPro(DescAndMoveDir[][] descAndMD)
        {
            char cellStand = 'O', cellUp = '∧', cellDown = '∨', cellLeft = '<', cellRight = '>';
            char deadCell = '#', food = '*', empty = ' ', poison = '*';
            //DescAndMoveDir[][] descAndMD = universe.GetAllDescriptorsAndMoveDisp();


            string outputStr = fieldStr + '\n';
            for (int j = 0; j < height; j++)
            {
                outputStr += '|';
                for (int i = 0; i < width; i++)
                {
                    int descriptor = descAndMD[i][j].desc;
                    if (descriptor == 0)
                    {
                        outputStr += empty;
                    }
                    else if (descriptor == -1)
                    {
                        outputStr += food;
                    }
                    else if (descriptor == -2)
                    {
                        outputStr += deadCell;
                    }
                    else if (descriptor == -3)
                    {
                        outputStr += poison;
                    }
                    else
                    {
                        char cell = 'O';
                        switch (descAndMD[i][j].moveDir)
                        {
                            case MoveDirection.up:
                                cell = cellUp;
                                break;

                            case MoveDirection.down:
                                cell = cellDown;
                                break;

                            case MoveDirection.left:
                                cell = cellLeft;
                                break;

                            case MoveDirection.right:
                                cell = cellRight;
                                break;

                            default:
                                cell = cellStand;
                                break;
                        }
                        outputStr += cell;
                    }
                }
                outputStr += "|\n";

            }

            outputStr += fieldStr;

            return outputStr;
        }

        void WriteInfo()
        {
            string info = @"Info";
            long uTick = universe.GetTicksCount();

            info += string.Format("\nWidth: {0};", width);
            info += string.Format("\nHeight: {0};", height);
            info += string.Format("\nStatus: {0};", status);
            info += string.Format("\nTick number: {0};", uTick);

            object[] mostFit = universe.GetMostFitCell();
            if (mostFit == null)
                info += "\nNone cell;";
            else
            {
                Cell cell = (Cell)mostFit[0];
                int cellsCount = (int)mostFit[1];
                info += string.Format("\nCells count: {0};\nMost fit genome({1} cells):", universe.GetCellsCount(), cellsCount);

                info += string.Format("\n    descriptor: {0};\n    hunger = {1};\n    aggression: {2};\n    friendly: {3};\n    Reproduction: {4};\n    poisonAddiction: {5}",
                    cell.GetDesc(),
                    cell.GetGenome().GetHunger(),
                    cell.GetGenome().GetAggression(),
                    cell.GetGenome().GetFriendly(),
                    cell.GetGenome().GetReproduction(),
                    cell.GetGenome().GetPoisonAddiction()
                    );


                border = Color.FromArgb(cell.GetDesc());
            }

            infoLabel.Invoke(new Action(() =>
            {
                infoLabel.Text = info;
            }));
        }

        public void SetTicksPerSecond(float value)
        {
            loopWorker.SetUniversePause((int)(1 / value * 1000));
        }
        public void SetFramesPerSecond(float value)
        {
            loopWorker.SetFramePause((int)(1 / value * 1000));
        }
        public void SetPriority(ThreadPriority threadPriority)
        {
            loopWorker.SetThreadPriority(threadPriority);
        }

        public void StartSimulation()
        {
            loopWorker.Start(threadMood);
            status = @"running";
        }
        public void PauseSimulation()
        {
            loopWorker.Stop();
            status = @"paused";
            WriteInfo();
        }

        public void SetThreadMood(bool asyncMood)
        {
            threadMood = asyncMood;
            PauseSimulation();
            StartSimulation();
        }

        class LoopWorker
        {
            TextBoxOutputManager om;
            Thread universeUpdThread, frameUpdThread, singleUpdThread, singleInvokerThread, universeInvokerThread, frameInvokerThread;
            int universePause, framePause;
            int ticksPerFrame;
            ThreadPriority threadPriority;
            Action invokedFunc;

            public LoopWorker(TextBoxOutputManager textBoxOutputManager)
            {
                om = textBoxOutputManager;
                SetThreadPriority(ThreadPriority.Normal);
                SetUniversePause(0);
                SetFramePause(0);
            }

            public void Dispose()
            {
                Stop();
                om = null;
            }

            public void SetThreadPriority(ThreadPriority threadPriority)
            {
                this.threadPriority = threadPriority;
            }

            public void SetUniversePause(int value)
            {
                if (value < 0)
                    return;
                universePause = value;
                if (universePause <= 0)
                    ticksPerFrame = 1000;
                else
                    ticksPerFrame = framePause / universePause;
                if (ticksPerFrame < 1)
                    ticksPerFrame = 1;
                
            }

            public void SetFramePause(int value)
            {
                if (value < 0)
                    return;
                framePause = value;
                if (universePause <= 0)
                    ticksPerFrame = 1000;
                else
                    ticksPerFrame = framePause / universePause;
                if (ticksPerFrame < 1)
                    ticksPerFrame = 1;
            }

            public void Start(bool asyncMood)
            {
                Stop();
                if (asyncMood)
                {
                    Action defActU = new Action(UniverseUpdateLoop);
                    Action defActF = new Action(FrameUpdateLoop);
                    Action invokedActU = null;
                    Action invokedActF = null;
                    universeUpdThread = new Thread(() =>
                    {
                        Thread.CurrentThread.Priority = threadPriority;
                        try
                        {
                            while (universeUpdThread == Thread.CurrentThread)
                            {
                                if (invokedActU != null)
                                    invokedActU();
                                invokedActU = null;
                            }
                        }
                        catch { }
                    });
                    universeUpdThread.Start();
                    frameUpdThread = new Thread(() =>
                    {
                        Thread.CurrentThread.Priority = threadPriority;
                        try
                        {
                            while (frameUpdThread == Thread.CurrentThread)
                            {
                                if (invokedActF != null)
                                {
                                    invokedActF();
                                    invokedActF = null;
                                }
                            }
                        }
                        catch { }
                    });
                    frameUpdThread.Start();



                    universeInvokerThread = new Thread(() =>
                    {
                        //Thread.CurrentThread.IsBackground = false;
                        //Thread.CurrentThread.Priority = threadPriority;
                        while (universeInvokerThread == Thread.CurrentThread)
                        {
                            invokedActU = defActU;
                            Thread.Sleep(universePause);
                        }

                    });
                    universeInvokerThread.Start();

                    frameInvokerThread = new Thread(() =>
                    {
                        //Thread.CurrentThread.IsBackground = false;
                        //Thread.CurrentThread.Priority = threadPriority;
                        while (frameInvokerThread == Thread.CurrentThread)
                        {
                            invokedActF = defActF;
                            Thread.Sleep(framePause);
                        }
                    });
                    frameInvokerThread.Start();
                }
                else
                {
                    Action defAct = new Action(SingleUpdateLoop);
                    Action invokedAct = null;

                    singleUpdThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = false;
                        Thread.CurrentThread.Priority = threadPriority;
                        try
                        {
                            while (singleUpdThread == Thread.CurrentThread)
                            {
                                if (invokedAct != null)
                                {
                                    invokedAct();
                                    invokedAct = null;
                                }
                            }
                        }catch { }
                    });
                    singleUpdThread.Start();

                    singleInvokerThread = new Thread(() =>
                    {
                        //Thread.CurrentThread.Priority = threadPriority;
                        while (singleInvokerThread == Thread.CurrentThread)
                        {
                            invokedAct = defAct;
                            Thread.Sleep(framePause);
                        }
                    });
                    singleInvokerThread.Start();

                }
            }

            public void Stop()
            {
                List<Thread> bufThreadList = new List<Thread>();
                bufThreadList.Add(singleUpdThread);
                bufThreadList.Add(universeUpdThread);
                bufThreadList.Add(frameUpdThread);
                bufThreadList.Add(universeInvokerThread);
                bufThreadList.Add(frameInvokerThread);
                bufThreadList.Add(singleInvokerThread);

                universeUpdThread = null;
                universeInvokerThread = null;
                frameUpdThread = null;
                frameInvokerThread = null;
                singleUpdThread = null;
                singleInvokerThread = null;

                foreach (Thread thr in bufThreadList)
                {
                    try
                    {
                        //thr.Join(50);
                        thr.Abort();
                    }
                    catch { }
                }
            }

            void SingleUpdateLoop()
            {

                if (om.universe.IsDisposed())
                {
                    om.Dispose();
                    return;
                }
                for(int i=0;i<ticksPerFrame;i++)
                {
                    if (om.universe.GetTicksCount() % 5 == 1)
                        om.universe.GetMostFitCell();
                    om.universe.DoUniverseTick();
                }
                om.WriteInfo();
                if (UniverseConsts.DrawMoveDirections)
                {
                    DescAndMoveDir[][] damd = om.universe.GetAllDescriptorsAndMoveDisp();
                    om.DrawPicturePro(damd);
                }
                else
                {
                    int[][] desc= om.universe.GetAllDescriptors();
                    om.DrawPictureDef(desc);
                }

            }
            void UniverseUpdateLoop()
            {
                if (om.universe.IsDisposed())
                {
                    om.Dispose();
                    return;
                }
                if (om.universe.GetTicksCount() % 5 == 1)
                    om.universe.GetMostFitCell();
                om.universe.DoUniverseTick();
                if (invokedFunc!=null)
                    invokedFunc();
            }
            void FrameUpdateLoop()
            {
                if (om.universe.IsDisposed())
                {
                    om.Dispose();
                    return;
                }

                om.WriteInfo();
                if (UniverseConsts.DrawMoveDirections)
                {
                    bool invoked = false;
                    DescAndMoveDir[][] damd = null;
                    invokedFunc = delegate
                    {
                        damd = om.universe.GetAllDescriptorsAndMoveDisp();
                        invoked = true;
                    };
                    while (!invoked)
                        Thread.Sleep(5);
                    
                    invokedFunc = null;
                    om.DrawPicturePro(damd);
                }
                else
                {
                    bool invoked=false;
                    int[][] desc=null;
                    invokedFunc = delegate
                    {
                        desc = om.universe.GetAllDescriptors();
                        invoked = true;
                    };
                    while (!invoked)
                        Thread.Sleep(5);
                    invokedFunc = null;
                    om.DrawPictureDef(desc);
                }
            }

            //void UniverseUpdateLoop()
            //{
            //    try
            //    {
            //        if (om.universe.IsDisposed())
            //        {
            //            om.Dispose();
            //            return;
            //        }

            //        om.universe.DoUniverseTick();

            //        blockedFrameUpdThread = false;
            //        while (blockedUniverseUpdThread)
            //            Thread.Sleep(5);
            //        Thread.Sleep(universePause);
            //    }
            //    catch { }
            //}
            //void FrameUpdateLoop()
            //{
            //    try
            //    {
            //        if (om.universe.IsDisposed())
            //        {
            //            om.Dispose();
            //            return;
            //        }

            //        blockedUniverseUpdThread = true;
            //        blockedFrameUpdThread = true;
            //        while (blockedFrameUpdThread)
            //            Thread.Sleep(5);

            //        om.WriteInfo();
            //        if (UniverseConsts.DrawMoveDirections)
            //        {
            //            DescAndMoveDir[][] damd = om.universe.GetAllDescriptorsAndMoveDisp();
            //            blockedUniverseUpdThread = false;
            //            om.DrawPicturePro(damd);
            //        }
            //        else
            //        {
            //            int[][] desc = om.universe.GetAllDescriptors();
            //            blockedUniverseUpdThread = false;
            //            om.DrawPictureDef(desc);
            //        }

            //        Thread.Sleep(framePause);
            //    }
            //    catch { }

            //}

        }
    

      


    }

    public static class SuspendControlUpdate
    {
        private const int WM_SETREDRAW = 0x000B;

        public static void Suspend(Control control)
        {
            control.Invoke(new Action(() =>
            {
                Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                    IntPtr.Zero);

                NativeWindow window = NativeWindow.FromHandle(control.Handle);
                window.DefWndProc(ref msgSuspendUpdate);
            }));
        }

        public static void Resume(Control control)
        {
            control.Invoke(new Action(() =>
            {
                // Create a C "true" boolean as an IntPtr
                IntPtr wparam = new IntPtr(1);
                Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                    IntPtr.Zero);

                NativeWindow window = NativeWindow.FromHandle(control.Handle);
                window.DefWndProc(ref msgResumeUpdate);

                control.Invalidate();
            }));
        }
    }
}
