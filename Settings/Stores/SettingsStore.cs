using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Settings.Memento;

namespace Settings.Stores
{
    public interface ISettingsStore<T> : INotifyPropertyChanged 
        where T : class, ISettingsMemento
    {
        #region DEFAULTS

        /// <summary>
        /// Лист заводских настроек
        /// </summary>
        List<T> Defaults { get; }

        #endregion

        #region USERS

        /// <summary>
        /// Лист пользовательских настроек
        /// </summary>
        List<T> Users { get; }

        #endregion

        #region ADD TO USERS

        /// <summary>
        /// Добавить настройку в магазин
        /// </summary>
        /// <param name="item">Новая настройка</param>
        void Add(T item);

        #endregion

        #region REMOVE

        /// <summary>
        /// Удалить настройку из магазина
        /// </summary>
        /// <param name="item">Настройка для удаления</param>
        void Remove(T item);

        #endregion

        #region EXPORT

        /// <summary>
        /// Экспортирует список настроек в файл
        /// </summary>
        /// <param name="path">Путь к файлу сохранения настроек</param>
        /// <param name="mementos">Настройки для сохранения</param>
        void Export(string path, IList<T> mementos);

        /// <summary>
        /// Экспортирует все настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу сохранения настроек</param>
        void ExportAll(string path);

        #endregion

        #region GET FROM FILE

        /// <summary>
        /// Возвращает список настроек из файла
        /// </summary>
        /// <param name="path">Путь к файлу настроек</param>
        List<T> GetMementosFromFile(string path);

        #endregion

        #region GET BY TEMPLATE

        /// <summary>
        /// Возвращает мементо по шаблону (эквивалентные шаблонному)
        /// </summary>
        /// <param name="memento">Шаблонное мементо</param>
        T GetByTemplate(T memento);

        /// <summary>
        /// Возвращает мементо по шаблону но без учета названия
        /// </summary>
        /// <param name="memento">Шаблонное мементо</param>
        T GetByTemplateAnotherName(T memento);

        #endregion

        #region CHECK FOR CONFLICTS

        /// <summary>
        /// Проверяет список настроек на наличие конфликтов с существующими в списке настроек
        /// </summary>
        /// <param name="items">Настройки</param>
        /// <returns>Возвращает конфликтыне настройки и "чистые" - с которыми нет конфликтов</returns>
        (List<T> conflicts, List<T> clears) CheckForConflicts(List<T> items);

        #endregion

        #region PROCESS CONFLICTS AND CLEARS

        /// <summary>
        /// Разрешает конфликты на основе выбранного режима
        /// "Чистые" просто добавляются в магазин
        /// </summary>
        /// <param name="conflicts">Конфликтные настройки и режим разрешения конфликта</param>
        /// <param name="clears">Чистые настройки - новые</param>
        //void ProcessConflictsAndClears(Dictionary<T, ConflictMode> conflicts, List<T> clears);

        void ProcessConflicts(Dictionary<T, ConflictMode> conflicts);

        void ProcessClears(List<T> clears);

        #endregion

        #region EVENTS

        event EventHandler<T> Added;
        event EventHandler<T> Removed;

        #endregion
    }

    public class SettingsStore<T> : ISettingsStore<T> where T : class, ISettingsMemento
    {
        #region DEFAULTS

        public List<T> Defaults { get; private set; }

        #endregion

        #region USERS

        public List<T> Users { get; private set; }

        #endregion

        #region EVENTS

        public event EventHandler<T> Added;
        public event EventHandler<T> Removed;

        #endregion

        private readonly ISettingFileSerializer _settingFileSerializer;

        public SettingsStore(ISettingFileSerializer settingFileSerializer)
        {
            _settingFileSerializer = settingFileSerializer;

            Defaults = new List<T>();
            Users = new List<T>();

            var defaults = _settingFileSerializer.GetDefaults<T>();
            if (defaults != null)
            {
                foreach (var @default in defaults)
                {
                    @default.IsAnisoprintApproved = true;
                    @default.PropertyChanged += ItemOnPropertyChanged;
                    @default.PropertyChanging += ItemOnPropertyChanging;
                    Defaults.Add(@default);
                }
            }
            
            var users = _settingFileSerializer.GetUsers<T>();
            if (users != null)
            {
                foreach (var user in users)
                {
                    user.IsAnisoprintApproved = false;
                    user.PropertyChanged += ItemOnPropertyChanged;
                    user.PropertyChanging += ItemOnPropertyChanging;
                    Users.Add(user);
                }
            }
        }

        #region ON PROPERTY CHANGED

        [ExcludeFromCodeCoverage]
        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var memento = (T)sender;
            _settingFileSerializer.SaveMemento(memento);
        }

        #endregion

        #region ON PROPERTY CHANGING

        [ExcludeFromCodeCoverage]
        private void ItemOnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            var memento = (T)sender;
            _settingFileSerializer.RemoveMemento(memento);
        }

        #endregion

        #region ADD TO USERS

        public void Add(T item)
        {
            //обновляем имя материала так, чтобы такое имя было новым, добавляя после имени (number)
            _settingFileSerializer.UpdateNameToUnique(item);
            item.IsAnisoprintApproved = false;

            //добавляем в словарь материалов сам материал и его мементо
            Users.Add(item);
            item.PropertyChanged += ItemOnPropertyChanged;
            item.PropertyChanging += ItemOnPropertyChanging;
            //сохраняем материал в файл
            _settingFileSerializer.SaveMemento(item);

            OnPropertyChanged("Users");
            Added?.Invoke(this, item);
        }

        #endregion

        #region REMOVE

        public void Remove(T item)
        {
            _settingFileSerializer.RemoveMemento(item);
            //находим в каком списке находится материал и удаляем из списка и файл
            if (Defaults.Contains(item))
            {
                Defaults.Remove(item);
                OnPropertyChanged("Defaults");
            }
            if (Users.Contains(item))
            {
                Users.Remove(item);
                OnPropertyChanged("Users");
            }
            Removed?.Invoke(this, item);
        }

        #endregion

        #region EXPORT

        public void Export(string path, IList<T> mementos)
        {
            if (mementos == null || mementos.Count == 0) return;
            _settingFileSerializer.ExportMementos(mementos, path);
        }

        #endregion

        #region EXPORT ALL

        public void ExportAll(string path)
        {
            Func<IEnumerable<T>> getByAll = () =>
            {
                if (Defaults.Count == 0 && Users.Count == 0) return null;
                if (Defaults.Count > 0 && Users.Count == 0) return Defaults;
                if (Defaults.Count == 0 && Users.Count > 0) return Users;
                return Defaults.Union(Users);
            };

            var mementos = getByAll();
            _settingFileSerializer.ExportMementos(mementos, path);
        }

        #endregion

        #region GET MEMENTOS FROM FILE

        public List<T> GetMementosFromFile(string path)
        {
            //импортируем мементо из файла
            List<T> mementos;
            try
            {
                mementos = _settingFileSerializer.Import<T>(path).ToList();
            }
            catch (JsonSerializationException e)
            {
                throw;
            }

            return mementos;
        }

        #endregion

        #region GET BY TEMPLATE

        public T GetByTemplate(T memento)
        {
            var inD = this.Defaults.FirstOrDefault(a => a.Equals(memento));
            if (inD != null) return inD;

            var inU = this.Users.FirstOrDefault(a => a.Equals(memento));
            return inU;
        }

        public T GetByTemplateAnotherName(T memento)
        {
            var inD = this.Defaults.FirstOrDefault(a => a.EqualsWithoutNameVersionApproved(memento));
            if (inD != null) return inD;

            var inU = this.Users.FirstOrDefault(a => a.EqualsWithoutNameVersionApproved(memento));
            if (inU != null) return inU;

            return null;
        }

        #endregion

        #region CHECK FOR CONFLICTS

        public (List<T> conflicts, List<T> clears) CheckForConflicts(List<T> items)
        {
            var conflicts = new List<T>();
            var clears = new List<T>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.TrueIsAnisoprintApproved) continue;

                    var hasSame = Users.Count(a => a.Name.Equals(item.Name) && a.Equals(item) == false) > 0;
                    if (hasSame) conflicts.Add(item);
                    else
                    {
                        var hasDuplicate = Users.Count(a => a.Name.Equals(item.Name) && a.Equals(item)) > 0;
                        if (!hasDuplicate) clears.Add(item);
                    }
                }
            }

            return (conflicts, clears);
        }

        #endregion

        #region PROCESS CONFLICTS AND CLEARS

        public void ProcessConflicts(Dictionary<T, ConflictMode> conflicts)
        {
            if (conflicts != null && conflicts.Any())
            {
                foreach (var conflictPair in conflicts)
                {
                    switch (conflictPair.Value)
                    {
                        case ConflictMode.FILL_OLD_FROM_NEW:
                            var same = Users.Single(a => a.Name.Equals(conflictPair.Key.Name));
                            same.FillFromAnother(conflictPair.Key);
                            break;
                        case ConflictMode.SAVE_NEW:
                            Add(conflictPair.Key);
                            break;
                    }
                }
            }
        }

        public void ProcessClears(List<T> clears)
        {
            if (clears != null)
            {
                foreach (var clear in clears)
                {
                    Add(clear);
                }
            }
        }

        #endregion

        #region ON PROPERTY CHANGED

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

    }



    public enum ConflictMode
    {
        SAVE_NEW,
        FILL_OLD_FROM_NEW,
    }
}