using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Windows.Forms;
using NAudio.Wave;

namespace Trayscout
{
    public class NightscoutClient : ApplicationContext
    {
        private Configuration _config;
        private HttpClient _client;
        private NotifyIcon _trayIcon;
        private Timer _timer;
        private DateTime _lastAlarm;
        private Bitmap _symbols;
        private GlucoseDiagram _diagram;
        private bool _diagramOpened;

        public NightscoutClient()
        {
            try
            {
                _symbols = GetSymbols();

                Ini ini = GetIni();
                _config = new Configuration(ini);

                _client = new HttpClient();
                _client.BaseAddress = new Uri(_config.BaseUrl);
                _client.DefaultRequestHeaders.Add("API-Secret", _config.ApiSecretHash);

                _lastAlarm = DateTime.MinValue;

                _timer = new Timer();
                _timer.Interval = 1000 * 60 * _config.UpdateInterval;
                _timer.Tick += (x, y) => Update(false);
                _timer.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

            Update(true);
        }

        private string GetAppDirectory()
        {
            return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);
        }

        private Bitmap GetSymbols()
        {
            string dir = GetAppDirectory();
            string img = Path.Combine(dir, "Symbols.png");

            if (!File.Exists(img))
                throw new Exception("File not found: " + Path.GetFileName(img));

            Bitmap result = (Bitmap)Image.FromFile(img);
            return result;
        }

        private Ini GetIni()
        {
            string dir = GetAppDirectory();
            string ini = Path.Combine(dir, "Config.ini");

            if (!File.Exists(ini))
                throw new Exception("File not found: " + Path.GetFileName(ini));

            Ini result = new Ini(ini);
            return result;
        }

        private void Update(bool firstRun)
        {
            Entry entry;
            try
            {
                entry = GetLatestEntry();
            }
            catch (Exception ex)
            {
                if (ex is NightscoutException || firstRun)
                {
                    while (ex.InnerException != null)
                        ex = ex.InnerException;
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
                else
                {
                    entry = new Entry(DateTime.Now, 0, _config.Unit, Trend.Flat);
                }
            }
            SetIcon(entry);
            SetAlarm(entry);
        }

        private Entry GetLatestEntry()
        {
            return GetLatestEntries(1).First();
        }

        private IList<Entry> GetLatestEntries(int count)
        {
            HttpResponseMessage requestResult = _client.GetAsync("entries?count=" + count).Result;
            if (!requestResult.IsSuccessStatusCode)
                throw new NightscoutException("Nightscout API: HTTP " + (int)requestResult.StatusCode + " " + requestResult.StatusCode);
            string content = requestResult.Content.ReadAsStringAsync().Result;
            IList<string> lines = content.Replace("\r\n", "\n").Split('\n').ToList();
            IList<Entry> entries = lines.Select(x => new Entry(x, _config.Unit)).Distinct().OrderByDescending(x => x.Timestamp).ToList();
            return entries;
        }

        private void SetIcon(Entry entry)
        {
            Icon ico = null;

            using (Bitmap bmp = new Bitmap(16, 16))
            {
                bmp.MakeTransparent();

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    _config.Style.SetSymbolColor(_symbols, _config.UseColor, _config.Low, _config.High, entry.Value);

                    int destOffsetX = (3 - entry.Digits.Count) * 5;

                    for (int i = 0; i < entry.Digits.Count; i++)
                    {
                        g.DrawImage(_symbols, new Rectangle(destOffsetX + i * 5, 8, 5, 8), new Rectangle(entry.Digits[i] * 5, 0, 5, 8), GraphicsUnit.Pixel);
                    }

                    if (entry.Trend != Trend.None)
                    {
                        g.DrawImage(_symbols, new Rectangle(10, 0, 5, 8), new Rectangle((int)entry.Trend, 0, 5, 8), GraphicsUnit.Pixel);
                        if (entry.Trend == Trend.DoubleDown || entry.Trend == Trend.DoubleUp)
                            g.DrawImage(_symbols, new Rectangle(5, 0, 5, 8), new Rectangle((int)entry.Trend, 0, 5, 8), GraphicsUnit.Pixel);
                    }
                }

                ico = Icon.FromHandle(bmp.GetHicon());
            }

            DisposeTrayIcon();
            _trayIcon = new NotifyIcon()
            {
                Icon = ico,
                ContextMenu = new ContextMenu(new MenuItem[] { new MenuItem("Exit", Exit) }),
                Visible = true
            };
            _trayIcon.Click += OpenGlucoseDiagram;
        }

        private void OpenGlucoseDiagram(object sender, EventArgs e)
        {
            if (!_diagramOpened)
            {
                _diagramOpened = true;
                IList<Entry> entries;
                try
                {
                    entries = GetLatestEntries(_config.TimeRange * 60);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                        ex = ex.InnerException;
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DateTime maxTimestamp = entries.Max(x => x.Timestamp);
                DateTime minTimestamp = maxTimestamp.AddHours(-(_config.TimeRange + 5));
                entries = entries.Where(x => x.Timestamp >= minTimestamp).ToList();
                _diagram = new GlucoseDiagram(_config, entries);
                _diagram.LostFocus += CloseDiagram;
                _diagram.Show();
                _diagram.Activate();
            }
        }

        private void SetAlarm(Entry entry)
        {
            if (_config.UseAlarm && (entry.Value >= _config.High || entry.Value <= _config.Low) && (entry.Value == 0 || DateTime.Now > _lastAlarm.AddMinutes(_config.AlarmInterval)))
            {
                string dir = GetAppDirectory();
                string alarm = Path.Combine(dir, "Alarm.mp3");

                if (File.Exists(alarm))
                {
                    WaveOut waveOut = new WaveOut();
                    AudioFileReader audioFileReader = new AudioFileReader(alarm);
                    waveOut.Init(audioFileReader);
                    waveOut.Play();
                    _lastAlarm = DateTime.Now;
                }
            }
        }

        private void CloseDiagram(object sender, EventArgs e)
        {
            _diagram.Close();
            _diagram.Dispose();
            _diagram = null;
            _diagramOpened = false;
        }

        private void DisposeTrayIcon()
        {
            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Click += OpenGlucoseDiagram;
                _trayIcon.Dispose();
                _trayIcon = null;
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            _timer?.Dispose();
            _client?.Dispose();
            _symbols?.Dispose();
            DisposeTrayIcon();
            Application.Exit();
        }
    }
}
