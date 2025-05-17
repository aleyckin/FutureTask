using Contracts.Dtos.MetricsDtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.PdfBuilder
{
    public class UserReportDocument : IDocument
    {
        private readonly List<ProjectInfo> _projectInfos;

        /// <summary>
        /// Конструктор принимает список информации по проектам пользователя
        /// </summary>
        public UserReportDocument(List<ProjectInfo> projectInfos)
        {
            _projectInfos = projectInfos;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Общие настройки страницы
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(12));
                page.PageColor(Colors.White);

                // Заголовок документа
                page.Header().Text("Отчёт по пользователю").FontSize(20).Bold().AlignCenter();

                // Содержимое
                page.Content().PaddingVertical(10).Column(column =>
                {
                    // Для каждого проекта выводим блок с информацией
                    foreach (var project in _projectInfos)
                    {
                        // Название проекта и роль пользователя
                        column.Item().Text($"Проект: {project.Title} (Роль: {project.RoleOnProject})").Bold();

                        // Общее количество задач и процент завершённых
                        var percent = project.CountOfTasks > 0
                            ? Math.Round(project.CountOfCompetedTasks * 100.0 / project.CountOfTasks, 1)
                            : 0;
                        column.Item().Text($"Всего задач: {project.CountOfTasks}, Завершено: {project.CountOfCompetedTasks} ({percent}%)");

                        // Таблица задач проекта
                        column.Item().Table(table =>
                        {
                            // Определяем колонки: №, Название, Приоритет, Дата создания, Дата окончания, Статус
                            table.ColumnsDefinition(def =>
                            {
                                def.ConstantColumn(30);
                                def.RelativeColumn(2);
                                def.RelativeColumn(1);
                                def.RelativeColumn(1);
                                def.RelativeColumn(1);
                                def.RelativeColumn(1);
                            });

                            // Заголовок таблицы
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("#");
                                header.Cell().Element(CellStyle).Text("Заголовок");
                                header.Cell().Element(CellStyle).Text("Приоритет");
                                header.Cell().Element(CellStyle).Text("Создана");
                                header.Cell().Element(CellStyle).Text("Окончание");
                                header.Cell().Element(CellStyle).Text("Статус");
                            });

                            // Вывод каждой задачи
                            for (int i = 0; i < project.TaskInfos.Count; i++)
                            {
                                var t = project.TaskInfos[i];
                                table.Cell().Element(CellStyle).Text((i + 1).ToString());
                                table.Cell().Element(CellStyle).Text(t.Title);
                                table.Cell().Element(CellStyle).Text(t.Priority.ToString());
                                table.Cell().Element(CellStyle).Text(t.DateCreated.ToString("yyyy-MM-dd"));
                                table.Cell().Element(CellStyle).Text(t.DateEnd.ToString("yyyy-MM-dd"));
                                table.Cell().Element(CellStyle).Text(t.Status);
                            }

                            // Стиль ячеек
                            static IContainer CellStyle(IContainer cell) => cell.Border(1).Padding(4);
                        });

                        // Разделитель между проектами
                        column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    }
                });

                // Футер
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Сгенерировано ");
                    x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")).SemiBold();
                });
            });
        }
    }
}
