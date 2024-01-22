/*=========================================================================================
    File Name: doughnut.js
    Description: Chartjs simple doughnut chart
    ----------------------------------------------------------------------------------------
    Item Name: Chameleon Admin - Modern Bootstrap 4 WebApp & Dashboard HTML Template + UI Kit
    Version: 1.0
    Author: ThemeSelection
    Author URL: https://themeselection.com/
==========================================================================================*/

// Doughnut chart
// ------------------------------
$(window).on("load", function () {
    // Діаграми для загальної статистики
    createDoughnutChart("#colorsDoughnutChart", jsonData.mainColors, jsonData.mainColorsCount, "Найбільш знаходжувані кольори", true);
    createDoughnutChart("#objDoughnutChart", jsonData.mainObj, jsonData.mainObjCount, "Найбільш знаходжувані об'єкти", false);

    // Діаграми для статистики зареєстрованого користувача
    if (jsonData.mainColorsOneUser && jsonData.mainObjOneUser) {
        createDoughnutChart("#colorsUserDoughnutChart", jsonData.mainColorsOneUser, jsonData.mainColorsOneUserCount, "Ваші найбільш знаходжувані кольори", true);
        createDoughnutChart("#objUserDoughnutChart", jsonData.mainObjOneUser, jsonData.mainObjOneUserCount, "Ваші найбільш знаходжувані об'єкти", false);
    }
});

function createDoughnutChart(ctxSelector, labels, data, label, backgroundAvailable) {
    var ctx = $(ctxSelector);
    if (backgroundAvailable) {
        backgroundColors = labels.map(color => '#' + color);

    }
    else {
        backgroundColors = ['#9FBDBE', '#DAC8AE', '#CE8999', '#968F56', '#E3CA86']
    }

    var chartData = {
        labels: labels,
        datasets: [{
            label: label,
            data: data,
            backgroundColor: backgroundColors,
        }]
    };

    var config = {
        type: 'doughnut',
        options: getChartOptions(),
        data: chartData
    };

    new Chart(ctx, config);
}

function getChartOptions() {
    return {
        responsive: true,
        maintainAspectRatio: false,
        responsiveAnimationDuration: 500,
    };
}