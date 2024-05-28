<template>
    <button type="button" class="btn btn-secondary mt-1" @click.prevent="exportSession">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-download" viewBox="0 0 16 16">
            <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5"/>
            <path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708z"/>
        </svg>
        Export
    </button> 
</template>

<script>
export default {
    methods: {
        async exportSession() {
            await fetch(`${this.$root.apiUrl}/api/export`).then(response => {
                const blob = new Blob([response.data], { type: 'application/zip' });
                const link = document.createElement('a')
                link.href = URL.createObjectURL(blob)
                link.download = response.headers.get('content-disposition').split(';')[1].split('=')[1]
                link.click()
                URL.revokeObjectURL(link.href)
                console.log(response.headers.get('content-disposition').split(';')[1].split('=')[1])
            }).catch(console.error);
        }
    }
}
</script>