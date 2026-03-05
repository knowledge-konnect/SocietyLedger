import Navbar from '../components/layout/Navbar'
import { Link } from 'react-router-dom'

export default function FreeTrial() {
  return (
    <div className="min-h-screen bg-background">
      <Navbar variant="landing" />

      <main className="py-24">
        <div className="max-w-4xl mx-auto px-4">
          <div className="text-center mb-10">
            <h1 className="text-3xl md:text-4xl font-bold text-foreground">Try SocietyLedger free for 14 days</h1>
            <p className="text-muted-foreground max-w-2xl mx-auto mt-3">No credit card required. Set up your society, send the first bill, and see payments come in.</p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-8 items-start">
            <div className="p-8 bg-card border border-border rounded-xl shadow-sm text-center">
              <h2 className="text-lg font-semibold text-foreground mb-4">What you get</h2>
              <ul className="mx-auto max-w-md space-y-3 text-muted-foreground">
                <li className="flex items-center justify-center gap-3"><span className="text-primary">•</span> Full access to Professional features for 14 days</li>
                <li className="flex items-center justify-center gap-3"><span className="text-primary">•</span> Create bills, send email reminders, and track payments</li>
                <li className="flex items-center justify-center gap-3"><span className="text-primary">•</span> Invite admins and members, export reports</li>
                <li className="flex items-center justify-center gap-3"><span className="text-primary">•</span> No credit card required, cancel anytime</li>
              </ul>

              <div className="mt-8 grid grid-cols-1 sm:grid-cols-2 gap-3">
                <Link to="/signup" className="w-full inline-flex items-center justify-center px-6 py-3 rounded-lg bg-gradient-to-r from-primary to-secondary text-white font-semibold">Start Free Trial</Link>
                <Link to="/subscription" className="w-full inline-flex items-center justify-center px-6 py-3 rounded-lg border-2 border-border text-foreground">Compare Plans</Link>
              </div>
            </div>

            <div className="p-8 bg-background rounded-xl border border-border/50">
              <h3 className="text-lg font-semibold text-foreground mb-4">3 simple steps</h3>
              <ol className="list-decimal list-inside space-y-4 text-muted-foreground">
                <li>
                  Create your society — name, address and admin contact.
                </li>
                <li>
                  Add flats and members, or invite them by email.
                </li>
                <li>
                  Create your first bill and send reminders — then watch payments arrive.
                </li>
              </ol>

              <div className="mt-8">
                <p className="text-sm text-muted-foreground">Need help? <Link to="/suggestions" className="text-primary underline">Contact our team</Link> and we'll walk you through setup.</p>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  )
}
