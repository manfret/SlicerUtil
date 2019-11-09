using System.ComponentModel;

namespace Settings.Memento
{
    /// <summary>
    /// Интрейс мементо настроек
    /// </summary>
    public interface ISettingsMemento : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region NAME

        /// <summary>
        /// Название профиля
        /// </summary>
        string Name { get; set; }

        #endregion

        #region IS ANISOPRINT APPROVED

        /// <summary>
        /// Указывает является ли данный профиль одобренным компанией Anisoprint
        /// </summary>
        bool? IsAnisoprintApproved { get; set; }

        /// <summary>
        /// Указывает является ли данный профиль одобренным компанией Anisoprint
        /// </summary>
        bool TrueIsAnisoprintApproved { get; }

        #endregion

        #region EQUALS WITHOUT NAME

        /// <summary>
        /// Проверяет являются ли настройки идентичными, но без имени, версии и IsAnisoprintApproved
        /// </summary>
        /// <param name="other">Другая настройка для сравнения</param>
        /// <returns>True если настройки идентичны кроме имени, версии, IsAnisoprintApproved</returns>
        bool EqualsWithoutNameVersionApproved(ISettingsMemento other);

        #endregion

        #region FILL FROM ANOTHER

        /// <summary>
        /// Заполняет мементо значениями из другого мементов
        /// </summary>
        /// <param name="other">Другое мементо</param>
        void FillFromAnother(ISettingsMemento other);

        #endregion
    }
}