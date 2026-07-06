using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class RemoveHasDataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.DeleteData(
                table: "ExecutionResults",
                keyColumn: "ExecutionResultId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ExecutionResults",
                keyColumn: "ExecutionResultId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ExecutionResults",
                keyColumn: "ExecutionResultId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ExecutionResults",
                keyColumn: "ExecutionResultId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MessageSeverities",
                keyColumn: "MessageSeverityId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MessageSeverities",
                keyColumn: "MessageSeverityId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MessageSeverities",
                keyColumn: "MessageSeverityId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProcessStages",
                keyColumn: "ProcessStageId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProcessStages",
                keyColumn: "ProcessStageId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProcessStages",
                keyColumn: "ProcessStageId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProcessStages",
                keyColumn: "ProcessStageId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProcessorModels",
                keyColumn: "ProcessorModelId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProcessorModels",
                keyColumn: "ProcessorModelId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProcessorModels",
                keyColumn: "ProcessorModelId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SystemEnvironments",
                keyColumn: "SystemEnvironmentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SystemEnvironments",
                keyColumn: "SystemEnvironmentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SystemEnvironments",
                keyColumn: "SystemEnvironmentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SystemEnvironments",
                keyColumn: "SystemEnvironmentId",
                keyValue: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.InsertData(
                table: "ExecutionResults",
                columns: new[] { "ExecutionResultId", "ResultDescription", "ResultName" },
                values: new object[,]
                {
                    { 1, "Execution completed successfully", "Passed" },
                    { 2, "Execution encountered errors", "Failed" },
                    { 3, "Execution was aborted", "Aborted" },
                    { 4, "Execution is currently running", "InProgress" },
                });

            migrationBuilder.InsertData(
                table: "MessageSeverities",
                columns: new[] { "MessageSeverityId", "SeverityDescription", "SeverityName" },
                values: new object[,]
                {
                    { 1, "Critical compilation error", "Error" },
                    { 2, "Non-blocking issue", "Warning" },
                    { 3, "Informational notification", "Info" },
                });

            migrationBuilder.InsertData(
                table: "ProcessStages",
                columns: new[] { "ProcessStageId", "StageName" },
                values: new object[,]
                {
                    { 1, "Compile" },
                    { 2, "UnitTest" },
                    { 3, "CodeAnalysis" },
                    { 4, "Deploy" },
                });

            migrationBuilder.InsertData(
                table: "ProcessorModels",
                columns: new[] { "ProcessorModelId", "LogicalCores", "PhysicalCores", "ProcessorName" },
                values: new object[,]
                {
                    { 1, 32, 16, "AMD Ryzen 9 7950X" },
                    { 2, 32, 24, "Intel Core i9-13900K" },
                    { 3, 12, 6, "AMD Ryzen 5 5600X" },
                });

            migrationBuilder.InsertData(
                table: "SystemEnvironments",
                columns: new[] { "SystemEnvironmentId", "EnvironmentDetails", "EnvironmentName" },
                values: new object[,]
                {
                    { 1, null, "Windows 11 Pro (64-bit)" },
                    { 2, null, "Windows 10 Pro (64-bit)" },
                    { 3, null, "Ubuntu 24.04 LTS (64-bit)" },
                    { 4, null, "macOS Sequoia 15" },
                });
        }
    }
}
