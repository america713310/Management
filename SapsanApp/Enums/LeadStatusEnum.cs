namespace SapsanApp.Enums
{
    // Перечислитель лидов
    public enum LeadStatusEnum
    {
        // Новый лид
        NewLead = 1,
        // Некачественный лид
        LowQualityLead = 2,
        // Список ожидания
        WaitingList = 3,
        // Приглашен на тест
        Invited = 4,
        // Перезвонить
        Recall = 5,
        // Отказ
        Refusal = 6
    }
}
