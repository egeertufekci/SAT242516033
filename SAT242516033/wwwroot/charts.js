window.createChart = (canvasId, type, labels, data, label) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return; // Canvas yoksa hata verme

    // Varsa eski grafiği temizle
    if (window[canvasId] instanceof Chart) {
        window[canvasId].destroy();
    }

    // Renkler
    const colors = [
        'rgba(54, 162, 235, 0.7)',
        'rgba(255, 99, 132, 0.7)',
        'rgba(255, 206, 86, 0.7)',
        'rgba(75, 192, 192, 0.7)',
        'rgba(153, 102, 255, 0.7)',
        'rgba(255, 159, 64, 0.7)'
    ];

    window[canvasId] = new Chart(ctx, {
        type: type,
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                backgroundColor: colors,
                borderColor: colors.map(c => c.replace('0.7', '1')),
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { position: 'bottom' }
            }
        }
    });
};