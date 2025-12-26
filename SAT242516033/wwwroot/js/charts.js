window.drawStatsChart = (urun, siparis, musteri, kullanici) => {
    const ctx = document.getElementById('statsBarChart');
    if (!ctx) return;

    // Eğer daha önce çizilmiş bir grafik varsa onu yok et (yenileme yaparken çakışmasın)
    if (window.myChart) {
        window.myChart.destroy();
    }

    window.myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Ürünler', 'Siparişler', 'Müşteriler', 'Kullanıcılar'],
            datasets: [{
                label: 'Sistem Veri Dağılımı',
                data: [urun, siparis, musteri, kullanici],
                backgroundColor: [
                    'rgba(13, 110, 253, 0.7)',  // Mavi (Primary)
                    'rgba(25, 135, 84, 0.7)',   // Yeşil (Success)
                    'rgba(255, 193, 7, 0.7)',   // Sarı (Warning)
                    'rgba(220, 53, 69, 0.7)'    // Kırmızı (Danger)
                ],
                borderColor: [
                    '#0d6efd', '#198754', '#ffc107', '#dc3545'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            plugins: {
                legend: { display: false }
            }
        }
    });
}