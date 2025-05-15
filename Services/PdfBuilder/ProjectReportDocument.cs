using Contracts.Dtos.MetricsDtos;
using Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;

public class ProjectReportDocument : IDocument
{
    private readonly Project _project;
    private readonly int _countOfTasks;
    private readonly int _completedTasks;
    private readonly List<UserInfo> _usersInfo;

    public ProjectReportDocument(
        Project project,
        int countOfTasks,
        int completedTasks,
        List<UserInfo> usersInfo)
    {
        _project = project;
        _countOfTasks = countOfTasks;
        _completedTasks = completedTasks;
        _usersInfo = usersInfo;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(12));
                page.PageColor(Colors.White);

                page.Header()
                    .Text($"Отчёт по проекту: {_project.Name}")
                    .FontSize(20).Bold().AlignCenter();

                page.Content().PaddingVertical(10).Column(column =>
                {
                    // Общая статистика
                    column.Spacing(5);
                    column.Item().Text($"Дата: {DateTime.UtcNow:yyyy-MM-dd HH:mm}");
                    column.Item().Text($"Всего задач: {_countOfTasks}");
                    column.Item().Text($"Завершено задач: {_completedTasks}");
                    var overallPercent = _countOfTasks > 0
                        ? Math.Round(_completedTasks * 100.0 / _countOfTasks, 1)
                        : 0;
                    column.Item().Text($"Процент завершения: {overallPercent}%");

                    // Таблица задач по пользователям
                    column.Item().PaddingTop(15).Text("Задачи по пользователям:").Bold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(def =>
                        {
                            def.ConstantColumn(30);   // №
                            def.RelativeColumn(2);    // Email
                            def.RelativeColumn(1);    // Роль
                            def.RelativeColumn(1);    // Выполнено
                            def.RelativeColumn(1);    // Всего
                            def.RelativeColumn(1);    // %
                        });

                        // Заголовок
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("#");
                            header.Cell().Element(CellStyle).Text("Email");
                            header.Cell().Element(CellStyle).Text("Роль");
                            header.Cell().Element(CellStyle).Text("Выполнено");
                            header.Cell().Element(CellStyle).Text("Всего");
                            header.Cell().Element(CellStyle).Text("%");
                        });

                        // Строки
                        for (int i = 0; i < _usersInfo.Count; i++)
                        {
                            var u = _usersInfo[i];
                            var userPercent = u.CountOfTasks > 0
                                ? Math.Round(u.CompletedTasks * 100.0 / u.CountOfTasks, 1)
                                : 0;

                            table.Cell().Element(CellStyle).Text((i + 1).ToString());
                            table.Cell().Element(CellStyle).Text(u.Email);
                            table.Cell().Element(CellStyle).Text(u.Role.ToString());
                            table.Cell().Element(CellStyle).Text(u.CompletedTasks.ToString());
                            table.Cell().Element(CellStyle).Text(u.CountOfTasks.ToString());
                            table.Cell().Element(CellStyle).Text($"{userPercent}%");
                        }

                        static IContainer CellStyle(IContainer cell) => cell
                            .Border(1)
                            .Padding(4);
                    });
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Сгенерировано ");
                        x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")).SemiBold();
                    });
            });
    }
}
