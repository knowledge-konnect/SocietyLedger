import Navbar from '../components/layout/Navbar'
import { Mail, Info } from 'lucide-react'

export default function PaymentGateways() {
  return (
    <div className="min-h-screen bg-background">
      <Navbar variant="landing" />
      <main className="py-16">
        <div className="max-w-4xl mx-auto px-4">
          <h1 className="text-3xl font-bold text-foreground mb-4">Payment Collection</h1>
          <p className="text-muted-foreground mb-8">In our MVP, maintenance payments are collected offline by the society admin.</p>

          <div className="space-y-6">
            <div className="p-6 rounded-lg bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 flex gap-4">
              <Info className="w-6 h-6 text-blue-600 dark:text-blue-400 flex-shrink-0 mt-0.5" />
              <div>
                <h3 className="font-semibold text-blue-900 dark:text-blue-100 mb-2">How Payment Collection Works</h3>
                <p className="text-blue-800 dark:text-blue-300 text-sm mb-4">
                  The society admin collects maintenance payments directly from residents (cash or personal UPI). After collection, the admin manually enters payment details into the app for record-keeping and reporting purposes.
                </p>
                <ul className="text-sm text-blue-800 dark:text-blue-300 space-y-2 ml-4 list-disc">
                  <li>Admin collects payments offline (cash or personal UPI)</li>
                  <li>Admin records payment details in the app</li>
                  <li>App generates reports on collection status and pending dues</li>
                  <li>Data remains in the app for audit and analysis</li>
                </ul>
              </div>
            </div>

            <div className="p-6 rounded-lg bg-card border border-border">
              <h3 className="text-lg font-semibold text-foreground mb-4">Need More Information?</h3>
              <p className="text-muted-foreground mb-4">
                If you have questions about payment collection or need further assistance, please contact our support team.
              </p>
              <a
                href="mailto:support@societyledger.com?subject=Payment Collection Inquiry"
                className="inline-flex items-center gap-2 px-6 py-3 bg-indigo-600 dark:bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 dark:hover:bg-indigo-700 transition-colors font-medium"
              >
                <Mail className="w-4 h-4" />
                Email Support
              </a>
            </div>
          </div>
        </div>
      </main>
    </div>
  )
}
