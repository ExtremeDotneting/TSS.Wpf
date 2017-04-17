namespace TSS.Another
{
    enum LanguageHandlerCulture{ en, ru }

    /// <summary>
    /// This class I used for localizing application. Maybe there is an easier way, but I was too lazy to search it, I admit.
    /// <para></para>
    /// Этот класс я использовал для локализаии приложения. Может есть способ проще, но мне было лень разбиратся, признаю.
    /// </summary>
    class LanguageHandler
    {
        static LanguageHandler lh = new LanguageHandler(LanguageHandlerCulture.en);
        public static LanguageHandler GetInstance()
        {
            return lh;
        }
        public static void SetLanguage(LanguageHandlerCulture languageHandlerCulture)
        {
            lh = new LanguageHandler(languageHandlerCulture);
        }
        LanguageHandler(LanguageHandlerCulture language)
        {
           
            switch (language)
            {
                case LanguageHandlerCulture.en:
                    InitLanguage_En();
                    break;
                case LanguageHandlerCulture.ru:
                    InitLanguage_Ru();
                    break;
            }
        }
        
        //Default strings
        public const string WindowTitlesPrefix = "TSS - ";
        public string UniverseInfoStringFormatter = "Width:{0};\nHeight:{1};" +
            "\nTotal cell count:{2};\nTotal energy in universe:{3};\nTick number:{4};\nCount of cell types:{5}" +
            "\nMost fit cell info:";
        public string CellInfoStringFormatter = "Genome:\n\thunger:{0},\n\taggression:{1},\n\treproduction:{2}," +
                "\n\tcollectivity:{3},\n\tpoison addiction:{4},\n\tcorpse addiction:{5};\nCount with this genome:{6};\nColor:";
        public string CellInfoWindowTitle = WindowTitlesPrefix+ "Cell info";
        public string FieldRedactorSizeWarning = "Universe size is too big for this option.";
        public string ApplyButtonText = "Apply";
        public string ModalWindowAboutFoodPlace = WindowTitlesPrefix + "Food place redactor";
        public string ModalWindowAboutPoisonPlace = WindowTitlesPrefix + "Poison place redactor";
        public string CheckAllButton = "Check all";
        public string UncheckAllButton = "Uncheck all";
        public string WindowTitleOfUniverseConstsRedactor = WindowTitlesPrefix + "Universe consts redactor";
        public string IncorrectValueMsg = "Value at field \"{0}\" is incorrect. It mast be {1} type. ";
        public string IncorrectRangeMsg = "Value of \"{0}\" must be at range from {1} to {2}. ";
        public string ConstsUniverseFileCorrupted = "The configuration file is corrupted or can`t be opened. Overwrite it?";
        public string UniverseFileCorrupted = "The universe file is corrupted or can`t be opened.";
        public string CellsCountWarningMessage = "Can`t create cells. Count of cells is a natural number.";
        public string ResolutionWarningMessage = "Set width and height of resolution in range from 100 to 2000.";
        public string TitleOfUniverseOutputWindow = WindowTitlesPrefix + "Simulation";
        public string TabItem_SimulationInfoHeader="Simulation info";
        public string TabItem_GameHeader = "Game";
        public string TabItem_ControlsHeader = "Controls";
        public string ButtonClearFieldText ="Clear field";
        public string ButtonConstsRedactorText = "Consts redactor";
        public string ButtonFoodPlaceRedactotText = "Food place redactor";
        public string ButtonGenerateCellsText = "Generate cells";
        public string ButtonGenerateFoodOnAllText = "Generate food on all field";
        public string ButtonPauseText = "Pause";
        public string ButtonPosinPlaceRedactotText = "Poison place redactor";
        public string ButtonResetResolutionText = "Set resolution";
        public string ButtonSaveUniverseText = "Save universe";
        public string ButtonStarText = "Start";
        public string LabelCellsCountText = "Cells count";
        public string LabelDelayText = "Delay";
        public string LabelHeightText = "Height";
        public string LabelWidthText = "Width";
        public string ButtonOk = "OK";
        public string ButtonCancel = "Cancel";
        public string UniverseSizeWarning="Universe side must be at range from 2 to 1000.";
        public string OutOfMemory = "Out of memory. We need to close simulation.";
        public string ButtonCreateUniverse = "Create universe";
        public string ButtonLoadUniverse="Load universe";
        public string ButtonAbout = "About";
        public string AboutText = "You can read how to play in manual in game folder.\nAuthor contacts\n\tMail: yuram1box@gmail.com"+
            "\n\tVk: vk.com/yura_mysko \n\tTelephone: +380987739725";
        public string MainWindowTitle=WindowTitlesPrefix+ "Main window";
        //Default strings

        void InitLanguage_En()
        {
            UniverseInfoStringFormatter = "Width:{0};\nHeight:{1};" +
                "\nTotal cell count:{2};\nTotal energy in universe:{3};\nTick number:{4};\nCount of cell types:{5}" +
                "\nMost fit cell type:";
            CellInfoStringFormatter = "Genome:\n\thunger:{0},\n\taggression:{1},\n\treproduction:{2}," +
                     "\n\tcollectivity:{3},\n\tpoison addiction:{4},\n\tcorpse addiction:{5};\nCount with this genome:{6};\nColor:";
            CellInfoWindowTitle = WindowTitlesPrefix + "Cell info";
            FieldRedactorSizeWarning = "Universe size is too big for this option.";
            ApplyButtonText = "Apply";
            ModalWindowAboutFoodPlace = WindowTitlesPrefix + "Food place redactor";
            ModalWindowAboutPoisonPlace = WindowTitlesPrefix + "Poison place redactor";
            CheckAllButton = "Check all";
            UncheckAllButton = "Uncheck all";
            WindowTitleOfUniverseConstsRedactor = WindowTitlesPrefix + "Universe consts redactor";
            IncorrectValueMsg = "Value at field \"{0}\" is incorrect. It mast be {1} type. ";
            IncorrectRangeMsg = "Value of \"{0}\" must be at range from {1} to {2}. ";
            ConstsUniverseFileCorrupted = "The configuration file is corrupted or can`t be opened. Overwrite it?";
            UniverseFileCorrupted = "The universe file is corrupted or can`t be opened.";
            CellsCountWarningMessage = "Can`t create cells. Count of cells is a natural number < 50000.";
            ResolutionWarningMessage = "Set width and height of resolution in range from 100 to 2000.";
            TitleOfUniverseOutputWindow = WindowTitlesPrefix + "Simulation";
            TabItem_SimulationInfoHeader = "Simulation info";
            TabItem_GameHeader = "Game";
            TabItem_ControlsHeader = "Controls";
            ButtonClearFieldText = "Clear field";
            ButtonConstsRedactorText = "Consts redactor";
            ButtonFoodPlaceRedactotText = "Food place redactor";
            ButtonGenerateCellsText = "Generate cells";
            ButtonGenerateFoodOnAllText = "Generate food on all field";
            ButtonPauseText = "Pause";
            ButtonPosinPlaceRedactotText = "Poison place redactor";
            ButtonResetResolutionText = "Set resolution";
            ButtonSaveUniverseText = "Save universe";
            ButtonStarText = "Start";
            LabelCellsCountText = "Cells count";
            LabelDelayText = "Delay";
            LabelHeightText = "Height";
            LabelWidthText = "Width";
            ButtonOk = "Ok";
            ButtonCancel = "Cancel";
            UniverseSizeWarning = "Universe side must be at range from 2 to 1000.";
            OutOfMemory = "The application uses too much memory. We need to close simulation.";
            ButtonCreateUniverse = "Create universe";
            ButtonLoadUniverse = "Load universe";
            ButtonAbout = "About";
            AboutText = "You can read how to play in manual in game folder.\nAuthor contacts\n\tMail: yuram1box@gmail.com" +
                "\n\tVk: vk.com/yura_mysko \n\tTelephone: +380987739725";
            MainWindowTitle=WindowTitlesPrefix + "Main window";
        }
        void InitLanguage_Ru()
        {
            UniverseInfoStringFormatter = "Ширина:{0};\nДлина:{1};" +
                "\nКлеток на поле:{2};\nВсего энергии во вселенной:{3};\nНомер тика:{4};\nВсего типов клеток:{5}" +
                "\nСамый успешный тип клеток:";
            CellInfoStringFormatter = "Геном:\n\tГолод:{0},\n\tАгрессия:{1},\n\tРепродуктивность:{2}," +
                     "\n\tКоллективность:{3},\n\tТяга к яду:{4},\n\tТяга к мертвым клеткам:{5};\nКоличество клеток с этим геномом:{6};\nЦвет:";
            CellInfoWindowTitle = WindowTitlesPrefix + "Информация ок клетке";
            FieldRedactorSizeWarning = "Размер вселенной слишком большой чтобы использовать эту функцию.";
            ApplyButtonText = "Подтвердить";
            ModalWindowAboutFoodPlace = WindowTitlesPrefix + "Редактор размещения еды";
            ModalWindowAboutPoisonPlace = WindowTitlesPrefix + "Редактор размещения  яда";
            CheckAllButton = "Отметить все";
            UncheckAllButton = "Убрать отмеченное";
            WindowTitleOfUniverseConstsRedactor = WindowTitlesPrefix + "Редактор констант вселенной";
            IncorrectValueMsg = "Значение поля \"{0}\" некорректно. Оно должно иметь тип {1}. ";
            IncorrectRangeMsg = "Значение поля \"{0}\" должно быть числом в промежутке от {1} до {2}. ";
            ConstsUniverseFileCorrupted = "Файл настроек поврежден или не может быть открыт. Перезаписать его?";
            UniverseFileCorrupted = "Файл со вселенной поврежден или не может быть открыт.";
            CellsCountWarningMessage = "Создание клеток невозможно. Количество клеток - натуральное число  < 50000.";
            ResolutionWarningMessage = "Ширина и длина должно быть числом в промежутке от 100 до 2000.";
            TitleOfUniverseOutputWindow = WindowTitlesPrefix + "Симуляция";
            TabItem_SimulationInfoHeader = "Статистика";
            TabItem_GameHeader = "Игра";
            TabItem_ControlsHeader = "Управление";
            ButtonClearFieldText = "Очистить поле";
            ButtonConstsRedactorText = "Редактор констант";
            ButtonFoodPlaceRedactotText = "Редактор размещения еды";
            ButtonGenerateCellsText = "Сгенерировать клетки";
            ButtonGenerateFoodOnAllText = "Заполнить поле едой";
            ButtonPauseText = "Пауза";
            ButtonPosinPlaceRedactotText = "Редактор размещения яда";
            ButtonResetResolutionText = "Сменить разрешение";
            ButtonSaveUniverseText = "Сохранить вселенную";
            ButtonStarText = "Старт";
            LabelCellsCountText = "Кол. клеток";
            LabelDelayText = "Задержка";
            LabelHeightText = "Длина";
            LabelWidthText = "Ширина";
            ButtonOk = "Ок";
            ButtonCancel = "Отмена";
            UniverseSizeWarning = "Сторона вселенной должна быть в промежутке от 2 до 1000.";
            OutOfMemory = "Приложение использует слишком много оперативной памяти. Мы вынуждены остановить симуляцию.";
            ButtonCreateUniverse = "Создать вселенную";
            ButtonLoadUniverse = "Загрузить вселенную";
            ButtonAbout = "Справка";
            AboutText = "Вы можете прочитать информацию об игре в мануале в папке с игрой.\nСвязь с автором\n\tПочта: yuram1box@gmail.com" +
                "\n\tВк: vk.com/yura_mysko \n\tТелефон: +380987739725";
            MainWindowTitle = WindowTitlesPrefix + "Главное окно";
        }
    }
}
