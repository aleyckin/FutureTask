using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TaskMetrics
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Если у вас есть авторизованный пользователь:
        public Guid? UserId { get; set; }

        public double OpenToRec { get; set; }

        /// <summary>
        /// Время ожидания ответа ИИ (мс)
        /// </summary>
        public double RecResponseTime { get; set; }

        /// <summary>
        /// Время пользователя на доработку после рекомендаций (мс)
        /// </summary>
        public double RecToSave { get; set; }

        /// <summary>
        /// Общее время создания задачи (мс)
        /// </summary>
        public double TotalTime { get; set; }

        /// <summary>
        /// Флаг: использовал ли пользователь рекомендации
        /// </summary>
        public bool UsedRecommendations { get; set; }

        /// <summary>
        /// Момент, когда были отправлены метрики
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
