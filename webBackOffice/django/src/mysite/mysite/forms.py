from django import forms

class blackform(forms.Form):
    id = forms.IntegerField()
    message = forms.CharField(widget=forms.Textarea)
    
    def clean_message(self):
        message = self.cleaned_data['message']
        if message=='':
            raise forms.ValidationError("at Lest 4 Worls")
        return message
        
    def clean_id(self):
        id = self.cleaned_data['id']
        if(id < 0):
            raise forms.ValidationError('this must be > 0')
        return id

