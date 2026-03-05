import Navbar from '../components/layout/Navbar'
import { Mail, ArrowRight } from 'lucide-react'

export default function Suggestions() {
  return (
    <div className="min-h-screen bg-background">
      <Navbar variant="landing" />
      <main className="py-16">
        <div className="max-w-3xl mx-auto px-4">
          <h1 className="text-3xl font-bold text-foreground mb-4">Support & Feedback</h1>
          <p className="text-slate-600 dark:text-slate-400 mb-8">We'd love to hear from you. Please reach out with feedback, suggestions, or any issues.</p>

          <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-lg p-8 space-y-6">
            <div className="flex items-start gap-4">
              <div className="w-12 h-12 bg-indigo-100 dark:bg-indigo-900/30 rounded-lg flex items-center justify-center flex-shrink-0">
                <Mail className="w-6 h-6 text-indigo-600 dark:text-indigo-400" />
              </div>
              <div className="flex-1">
                <h2 className="text-xl font-semibold text-foreground mb-2">Email Support</h2>
                <p className="text-slate-600 dark:text-slate-400 mb-4">
                  Send us your feedback, suggestions, or report any issues directly to our support team.
                </p>
                <a
                  href="mailto:support@societyledger.com?subject=Feedback for SocietyLedger"
                  className="inline-flex items-center gap-2 px-6 py-3 bg-indigo-600 dark:bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 dark:hover:bg-indigo-700 transition-colors font-medium group"
                >
                  Send Email to support@societyledger.com
                  <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
                </a>
              </div>
            </div>

            <div className="border-t border-slate-200 dark:border-slate-800 pt-6">
              <h3 className="font-semibold text-foreground mb-4">Response Time</h3>
              <p className="text-slate-600 dark:text-slate-400">
                We respond to all support emails within 24 hours during business days.
              </p>
            </div>
          </div>
        </div>
      </main>
    </div>
  )
}
